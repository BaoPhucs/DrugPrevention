using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using DrugPreventionAPI.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DrugPreventionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserManagementRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _configuration;
        public AuthController(IAuthRepository authRepository, IMapper mapper, IEmailService emailService, IAdminRepository adminRepository, IUserManagementRepository userRepository , IConfiguration configuration)
        {
            _authRepository = authRepository;
            _mapper = mapper;
            _emailService = emailService;
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                return BadRequest("Email and password are required");

            var user = await _authRepository.LoginAsync(loginRequest.Email, loginRequest.Password);
            if (user == null)
                return Unauthorized("Invalid email or password");

            // 1) Chuẩn bị các claim
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role ?? "Member"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // 2) Lấy cấu hình JWT từ appsettings
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyBytes = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]));

            // 3) Tạo token
            var jwt = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

            // 4) Trả về kèm tiền tố "Bearer "
            return Ok(new
            {
                token = $"Bearer {tokenString}",
                expires = expires
            });
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDTO googleRequest)
        {
            if (string.IsNullOrWhiteSpace(googleRequest.GoogleToken))
                return BadRequest("Google token is required");

            var user = await _authRepository.AuthenticateGoogleAsync(googleRequest.GoogleToken);
            if (user == null)
                return Unauthorized("Invalid Google token");

            // 1) Tạo danh sách claims
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role,               user.Role ?? "Member"),
        new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
    };

            // 2) Lấy config và ký token
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyBytes = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
                                                     SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(
                                double.Parse(jwtSettings["ExpireMinutes"]));

            // 3) Tạo JWT
            var jwt = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

            // 4) Trả về kèm tiền tố "Bearer "
            return Ok(new
            {
                token = $"Bearer {tokenString}",
                expires = expires
            });
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
                Role = "Member", // Giá trị mặc định từ model
                EmailVerified = false, // Giả sử đã xác minh
                CreatedDate = DateTime.UtcNow
            };

            if (!await _authRepository.RegisterAsync(user))
                return StatusCode(500, "Error registering user");

            // Sinh token xác thực và lưu (cần cài thêm method trong IAuthRepository)
            var token = await _authRepository.GenerateEmailVerificationTokenAsync(user.Email);

            // Gửi email xác thực
            var confirmUrl = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?token={token}";
            var html = $@"
                <p>Hello {user.Name},</p>
                <p>Please click <a href=""{confirmUrl}"">here</a> to verify your email.</p>
                <p>This link is valid for 24 hours.</p>";

            await _emailService.SendEmailAsync(
                user.Email,
                "DrugPrevention Email Authentication",
                html
            );

            return Accepted(new { Message = "Please check your email for verification." });
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Invalid token.");

            var ok = await _authRepository.ConfirmEmailAsync(token);
            if (!ok) return BadRequest("The authentication link is invalid or has expired.");

            var loginUrl = $"{Request.Scheme}://localhost:5173/login";
            return Redirect(loginUrl);
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
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            // 1) Sinh mật khẩu mới
            var newPwd = _authRepository.GenerateRandomPassword(8);
            var updated = await _authRepository.UpdatePasswordAsync(resetPasswordDTO.Email, newPwd);
            if (!updated) return NotFound("Email does not exist");

            // 2) Gửi email cho user
            var html = $"<p>Hello,</p>"
                     + $"<p>Your new password is: <strong>{newPwd}</strong></p>"
                     + "<p>Please login and change your password now.</p>";
            await _emailService.SendEmailAsync(resetPasswordDTO.Email,
                                              "Reset pasword DrugPrevention",
                                              html);

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
