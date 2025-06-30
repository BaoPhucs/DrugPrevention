using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using DrugPreventionAPI.Repositories;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
        private readonly FirebaseAuth _firebaseAuth;
        private readonly HttpClient _http;
        public AuthController(IAuthRepository authRepository, IMapper mapper, IEmailService emailService, IAdminRepository adminRepository, IUserManagementRepository userRepository, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _authRepository = authRepository;
            _mapper = mapper;
            _emailService = emailService;
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            _firebaseAuth = FirebaseAuth.DefaultInstance;
            _http = clientFactory.CreateClient();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                return BadRequest("Email and password are required");

            var user = await _authRepository.LoginAsync(loginRequest.Email, loginRequest.Password);
            if (user == null)
                return Unauthorized("Invalid credentials or account is locked.");

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
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
        {
            // 1) Basic validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!new EmailAddressAttribute().IsValid(dto.Email))
                return BadRequest("Invalid email format");

            if (dto.Password.Length < 6)
                return BadRequest("Password must be at least 6 characters");

            if (await _adminRepository.UserExistAsync(dto.Email))
                return BadRequest("Email already in use");

            // 2) Map to your User model
            var user = new User
            {
                Name = dto.Name,
                Dob = dto.Dob,
                Phone = dto.Phone,
                Email = dto.Email,
                Password = dto.Password,   // consider hashing!
                AgeGroup = dto.AgeGroup,
                Role = "Member",
                EmailVerified = false,
                CreatedDate = DateTime.UtcNow
            };

            // 3) Save to your own Users table
            var ok = await _authRepository.RegisterAsync(user);
            if (!ok)
                return StatusCode(500, "Could not create user");

            var userArgs = new UserRecordArgs()
            {
                Email = user.Email,
                EmailVerified = false,
                Password = user.Password
            };
            var fbUser = await _firebaseAuth.CreateUserAsync(userArgs);

            // 1) sinh link Firebase
            var actionSettings = new ActionCodeSettings
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/Auth/confirm-email",
                HandleCodeInApp = false
            };
            var firebaseLink = await _firebaseAuth.GenerateEmailVerificationLinkAsync(user.Email, actionSettings);

            // 2) parse ra oobCode
            var uri = new Uri(firebaseLink);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            var code = queryParams["oobCode"].ToString();

            // 3) build link về API của bạn
            var confirmLink = $"{Request.Scheme}://{Request.Host}/api/Auth/confirm-email?oobCode={code}";

            // 4) gửi email: show luôn mã và link đúng
            var html = $@"
<p>Xin chào {user.Name},</p>
<p>Vui lòng xác thực email bằng một trong hai cách:</p>
<ul>
  <li>Click <a href=""{confirmLink}"">vào đây</a> để tự động xác thực.</li>
  <li>Hoặc copy mã sau và paste vào form xác thực trên ứng dụng:</li>
</ul>
<p><strong>{code}</strong></p>
<p>Mã có hiệu lực trong 24h.</p>";
            await _emailService.SendEmailAsync(user.Email, "Xác thực email DrugPrevention", html);

            return Accepted(new { Message = "Đăng ký thành công. Vui lòng kiểm tra hộp thư để xác thực email." });
        }


        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailByLink([FromQuery] string? oobCode)
        {
            if (string.IsNullOrWhiteSpace(oobCode))
                return BadRequest("Thiếu mã xác thực (oobCode).");

            var ok = await _authRepository.ConfirmEmailAsync(oobCode);
            if (!ok)
                return BadRequest("Mã xác thực không hợp lệ hoặc đã hết hạn.");

            // Trả về một trang HTML đơn giản
            var html = @"
<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <title>Xác thực email</title>
</head>
<body>
  <h1>Email đã được xác thực thành công!</h1>
  <p>Bạn có thể đóng trang này và quay trở lại ứng dụng.</p>
</body>
</html>";
            return new ContentResult
            {
                ContentType = "text/html; charset=utf-8",
                StatusCode = 200,
                Content = html
            };
        }


        [HttpPost("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Gọi Firebase để confirm email
            var ok = await _authRepository.ConfirmEmailAsync(dto.OobCode);
            if (!ok)
                return BadRequest("Mã xác thực không hợp lệ hoặc đã hết hạn.");

            return Ok("Email đã được xác thực thành công!");
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
            var claims = User.Claims.Select(c => new
            {
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
    }
}
