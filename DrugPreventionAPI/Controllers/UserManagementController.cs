using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserManagementRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        public UserManagementController(IUserManagementRepository userRepository, IMapper mapper, IAdminRepository adminRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _adminRepository = adminRepository;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Email) || string.IsNullOrWhiteSpace(registerDto.Password) ||
                string.IsNullOrWhiteSpace(registerDto.Name) || registerDto.Dob == null)
            {
                return BadRequest("All required fields (email, password, name, dob) must be provided");
            }

            // Kiểm tra định dạng email
            if (!new EmailAddressAttribute().IsValid(registerDto.Email))
            {
                return BadRequest("Invalid email format");
            }

            // Kiểm tra độ dài mật khẩu
            if (registerDto.Password.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters long");
            }

            // Kiểm tra email đã tồn tại
            if (await _adminRepository.UserExistAsync(registerDto.Email))
            {
                return BadRequest("Email already exists");
            }

            // Tạo và ánh xạ thủ công sang User
            var user = new User
            {
                Name = registerDto.Name,
                Dob = registerDto.Dob,
                Phone = registerDto.Phone, // Null nếu chưa có
                Email = registerDto.Email,
                Password = registerDto.Password, // Lưu mật khẩu thô
                AgeGroup = registerDto.AgeGroup, // Null nếu chưa có
                Role = "member", // Giá trị mặc định từ model
                EmailVerified = true, // Giả sử đã xác minh
                CreatedDate = DateTime.UtcNow
            };

            // Lưu vào DB
            var result = await _userRepository.RegisterAsync(user);
            if (!result)
            {
                return StatusCode(500, "An error occurred while registering the user");
            }

            // Tạo URI cho resource mới
            var resourceUri = $"/api/User/{user.Id}";
            return Created(resourceUri, new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Dob = user.Dob,
                Phone = user.Phone, // Null nếu chưa có
                Email = user.Email,
                Password = user.Password, // Trả về mật khẩu thô (không an toàn)
                Role = user.Role,
                AgeGroup = user.AgeGroup, // Null nếu chưa có
                ProfileData = user.ProfileData, // Null nếu chưa có
                EmailVerified = user.EmailVerified,
                CreatedDate = user.CreatedDate
            });
        }




        [HttpPut("update-info")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userDto)
        {
            if (userDto == null || userDto.Id <= 0)
            {
                return BadRequest("Invalid user data");
            }
            var user = _mapper.Map<User>(userDto);
            var result = await _userRepository.UpdateAsync(user);
            if (!result)
            {
                return StatusCode(500, "An error occurred while updating the user");
            }
            return NoContent(); // Trả về 204 No Content nếu cập nhật thành công
        }

        //this function to get the current user information based on the JWT token
        [HttpGet("aboutMe")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claimValue, out var currentUserId) || currentUserId <= 0)
                return Unauthorized("Invalid user ID");
            var user = await _userRepository.GetCurrentUserAsync(currentUserId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);


        }
    }
}
