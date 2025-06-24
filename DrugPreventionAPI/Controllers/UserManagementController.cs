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
