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

        public async Task<User> AuthenticateGoogleAsync(string googleToken)
        {
            if (string.IsNullOrEmpty(googleToken))
            {
                Console.WriteLine("Google token is null or empty"); // Debug
                return null;
            }
            Console.WriteLine($"Received googleToken: {googleToken?.Substring(0, 50)}..."); // Debug
            try
            {
                var clientId = _configuration["GoogleAuth:ClientId"];
                Console.WriteLine($"Client ID: {clientId}"); // Debug
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });
                var email = payload.Email;
                Console.WriteLine($"Validated email: {email}"); // Debug
                var user = await _adminRepository.GetProfileAsync(email) ?? new User { Email = email, EmailVerified = true };
                if (user.Id == 0) await _userManagementRepository.RegisterAsync(user);
                return user;
            }
            catch (InvalidJwtException ex)
            {
                Console.WriteLine($"Invalid JWT: {ex.Message}"); // Debug
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Debug
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
