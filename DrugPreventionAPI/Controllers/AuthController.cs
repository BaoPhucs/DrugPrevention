using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepository, IMapper mapper)
        {
            _authRepository = authRepository;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Email and password are required");
            }

            var user = await _authRepository.LoginAsync(loginRequest.Email, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            // Tạo session cookie với vai trò từ DB
            var role = user.Role ?? "user".ToLower(); // Lấy vai trò từ DB, mặc định là "user"
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new { user.Id, user.Email, role }); // Trả về thông tin cơ bản
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest googleRequest)
        {
            if (string.IsNullOrWhiteSpace(googleRequest.GoogleToken))
            {
                return BadRequest("Google token is required");
            }

            var user = await _authRepository.AuthenticateGoogleAsync(googleRequest.GoogleToken);
            if (user == null)
            {
                return Unauthorized("Invalid Google token");
            }

            // Tạo session cookie với vai trò từ DB
            var role = user.Role ?? "user"; // Lấy vai trò từ DB
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new { user.Id, user.Email, role }); // Trả về thông tin cơ bản
        }

        [HttpPost("change-password")]
        [Authorize] // Yêu cầu người dùng đã đăng nhập
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (string.IsNullOrWhiteSpace(changePasswordDTO.Email) ||
                string.IsNullOrWhiteSpace(changePasswordDTO.OldPassword) ||
                string.IsNullOrWhiteSpace(changePasswordDTO.NewPassword))
            {
                return BadRequest("Email, old password, and new password must be provided");
            }

            if (changePasswordDTO.NewPassword.Length < 6)
            {
                return BadRequest("New password must be at least 6 characters long");
            }

            if (changePasswordDTO.NewPassword != changePasswordDTO.ConfirmNewPassword)
            {
                return BadRequest("New password and confirmation password do not match");
            }

            var result = await _authRepository.ChangePasswordAsync(
                changePasswordDTO.Email,
                changePasswordDTO.OldPassword,
                changePasswordDTO.NewPassword,
                changePasswordDTO.ConfirmNewPassword);

            if (!result)
            {
                return Unauthorized("Invalid email or old password");
            }

            return NoContent(); // Trả về 204 No Content nếu đổi mật khẩu thành công
        }

        [HttpPost("reset-password")]
        [Authorize]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (string.IsNullOrWhiteSpace(resetPasswordDTO.Email))
            {
                return BadRequest("Email must be provided");
            }

            var result = await _authRepository.ResetPasswordAsync(resetPasswordDTO.Email);
            if (!result)
            {
                return NotFound($"User with email {resetPasswordDTO.Email} not found");
            }

            return Ok("Password reset link sent to your email"); // Trả về thông báo thành công
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out successfully");
        }


        [HttpGet("check-auth")]
        [Authorize]
        public IActionResult CheckAuth()
        {
            var claims = User.Claims.Select(c => new {
                Type = c.Type,
                Value = c.Value
            }).ToList();

            return Ok(new
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                Claims = claims,
                IsAdmin = User.IsInRole("Admin, Manager"),
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()
            });
        }

        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return Forbid("Access denied. Admin role required.");
        }
    }
}
