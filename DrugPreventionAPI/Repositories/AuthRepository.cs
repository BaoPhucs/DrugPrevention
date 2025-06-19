using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IAdminRepository _adminRepository;
        private readonly IUserManagementRepository _userManagementRepository;
        private readonly DataContext _context;
        public AuthRepository(DataContext context, IConfiguration configuration, IAdminRepository adminRepository, IUserManagementRepository userManagementRepository)
        {
            _context = context;
            _configuration = configuration;
            _adminRepository = adminRepository;
            _userManagementRepository = userManagementRepository;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user == null || user.Password != password)
            {
                return null; // Authentication failed
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
                    await _userManagementRepository.RegisterAsync(user);
                }
                else
                {
                    // cập nhật các thông tin có thể thay đổi
                    var updated = false;
                    if (user.Name != fullName) { user.Name = fullName; updated = true; }
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

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
