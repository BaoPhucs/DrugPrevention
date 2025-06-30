using System.Text.Json;
using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace DrugPreventionAPI.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IAdminRepository _adminRepository;
        private readonly IUserManagementRepository _userManagementRepository;
        private readonly DataContext _context;
        private readonly FirebaseAuth _firebaseAuth;
        private readonly HttpClient _http;
        public AuthRepository(DataContext context, IConfiguration configuration, IAdminRepository adminRepository, IUserManagementRepository userManagementRepository, IHttpClientFactory httpFactory)
        {
            _context = context;
            _configuration = configuration;
            _adminRepository = adminRepository;
            _userManagementRepository = userManagementRepository;
            _firebaseAuth = FirebaseAuth.DefaultInstance;
            _http = httpFactory.CreateClient();
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user == null || user.Password != password)
            {
                return null; // Authentication failed
            }
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                Console.WriteLine("User is locked out until " + user.LockoutEnd.Value);
                return null; // User is locked out
            }

            return user; // Authentication successful
        }



        public async Task<User?> AuthenticateGoogleAsync(string googleToken)
        {
            if (string.IsNullOrEmpty(googleToken))
            {
                Console.WriteLine("Google token is null or empty");
                return null;
            }

            try
            {
                var clientId = _configuration["GoogleAuth:ClientId"];
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });

                // những trường bạn có thể lấy từ payload
                var email = payload.Email!;
                var fullName = payload.Name;         // họ tên đầy đủ

                // tìm user theo email
                var user = await _adminRepository.GetProfileAsync(email);

                if (user == null)
                {
                    // tạo mới
                    user = new User
                    {
                        Email = email,
                        Name = fullName,
                        Role = "Member",
                        EmailVerified = true,
                        CreatedDate = DateTime.UtcNow
                    };
                    await RegisterAsync(user);
                }
                else
                {
                    // cập nhật các thông tin có thể thay đổi
                    var updated = false;
                    //if (user.Name != fullName) { user.Name = fullName; updated = true; }
                    if (user.EmailVerified != true) { user.EmailVerified = true; updated = true; }
                    if (updated)
                        await _userManagementRepository.UpdateAsync(user);
                }

                return user;
            }
            catch (InvalidJwtException ex)
            {
                Console.WriteLine($"Invalid JWT: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating Google token: {ex}");
                return null;
            }
        }


        public async Task<bool> RegisterAsync(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword, string confirmPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.Password != oldPassword)
            {
                return false; // User not found or old password does not match
            }
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                return false; // New password is invalid
            }

            if (newPassword != confirmPassword)
            {
                return false; // New password and confirmation password do not match
            }
            user.Password = newPassword;
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return false; // User not found
            }
            string newPassword = GenerateRandomPassword(8); // Generate a new random password
            user.Password = newPassword; // Update the user's password

            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

        public string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            // Tìm user theo email
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return false;

            // Gán password mới (lưu ý: ở production bạn nên hash!)
            user.Password = newPassword;

            // Lưu thay đổi
            return await _context.SaveChangesAsync() > 0;
        }

        // 1) Sinh link xác thực email
        public async Task<string> GenerateEmailVerificationLinkAsync(string email)
        {
            // Uses the Firebase Admin SDK to create a 24-hour verification link
            var actionCodeSettings = new ActionCodeSettings()
            {
                Url = "https://localhost:7155/api/Auth/confirm-email", // optional: deep link target
                HandleCodeInApp = false,
                // You can restrict this link to only work for your Android/iOS apps:
                // AndroidPackageName = "...", IOSBundleId = "..."
            };
            return await FirebaseAuth.DefaultInstance
                                      .GenerateEmailVerificationLinkAsync(email, actionCodeSettings);
        }

        public async Task<bool> ConfirmEmailAsync(string oobCode)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={apiKey}";
            var payload = new
            {
                oobCode = oobCode,
                requestType = "VERIFY_EMAIL"
            };
            var res = await _http.PostAsJsonAsync(url, payload);
            if (!res.IsSuccessStatusCode)
                return false;

            // 1) Parse response để lấy email
            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("email", out var eProp))
                return false;
            var email = eProp.GetString();
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // 2) Cập nhật cột EmailVerified
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && user.EmailVerified == false)
            {
                user.EmailVerified = true;
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
