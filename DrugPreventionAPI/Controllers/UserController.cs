using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userRepository.LoginAsync(loginRequest.Email, loginRequest.Password);
            if (user == null) return Unauthorized("Invalid email or password");
            return Ok(new { user.Id, user.Email, user.Role }); // Trả về token trong thực tế
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest googleRequest)
        {
            var user = await _userRepository.AuthenticateGoogleAsync(googleRequest.GoogleToken);
            if (user == null) return Unauthorized("Invalid Google token");
            return Ok(new { user.Id, user.Email, user.Role }); // Trả về token trong thực tế
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
            if (await _userRepository.UserExistAsync(registerDto.Email))
            {
                return BadRequest("User with this email already exists");
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


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUserAsync();
            if (users == null || !users.Any())
            {
                return NotFound("No users found");
            }
            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(userDtos);
        }
    }
} 
