using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        public AdminController(IAdminRepository adminRepository, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        [HttpGet("get-users")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminRepository.GetAllUserAsync();
            if (users == null || !users.Any())
            {
                return NotFound("No users found");
            }
            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(userDtos);
        }

        [HttpGet("get-user/{userId}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _adminRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }

        [HttpGet("get-userProfile/{email}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> GetUserProfile(string email)
        {
            var user = await _adminRepository.GetProfileAsync(email);
            if (user == null)
            {
                return NotFound($"User with email {email} not found");
            }
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }


        [HttpDelete("delete-user/{userId}")]
        [Authorize(Roles = "Admin, Manager")] // Chỉ cho phép người dùng có vai trò admin xóa người dùng
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _adminRepository.DeleteAsync(userId);
            if (!result)
            {
                return NotFound($"User with ID {userId} not found");
            }
            return NoContent(); // Trả về 204 No Content nếu xóa thành công
        }

        [HttpPost("assign-role/{userId}")]
        [Authorize(Roles = "Admin, Manager")] // Chỉ cho phép người dùng có vai trò admin hoặc manager gán vai trò
        public async Task<IActionResult> AssignRole(int userId, [FromBody] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return BadRequest("Role must be provided");
            }
            var result = await _adminRepository.AssignRoleAsync(userId, role);
            if (!result)
            {
                return NotFound($"User with ID {userId} not found or role assignment failed");
            }
            return NoContent(); // Trả về 204 No Content nếu gán vai trò thành công
        }

        [HttpGet("get-users-by-role/{role}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return BadRequest("Role must be provided");
            }
            var users = await _adminRepository.GetUsersByRoleAsync(role);
            if (users == null || !users.Any())
            {
                return NotFound($"No users found with role {role}");
            }
            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(userDtos);
        }


        [HttpPut("update-user/{userId}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserDTO userDto)
        {
            if (userDto == null || userDto.Id != userId)
            {
                return BadRequest("Invalid user data");
            }
            var user = _mapper.Map<User>(userDto);
            user.Id = userId; // Đảm bảo ID của người dùng được cập nhật là ID đã cho
            var result = await _adminRepository.UpdateUserByAdminAsync(user);
            if (!result)
            {
                return NotFound($"User with ID {userId} not found or update failed");
            }
            return NoContent(); // Trả về 204 No Content nếu cập nhật thành công
        }
    }
}
