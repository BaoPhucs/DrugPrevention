using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> LoginAsync(string email, string password);
        Task<User> AuthenticateGoogleAsync(string googleToken); // Authenticates a user using Google OAuth
        Task<bool> RegisterAsync(User user);
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword, string confirmPassword); // Changes the password for a user
        Task<bool> ResetPasswordAsync(string email); // Resets the password for a user by email
        string GenerateRandomPassword(int length);
        Task<bool> UpdatePasswordAsync(string email, string newPassword);
        Task<string> GenerateEmailVerificationTokenAsync(string email);
        Task<bool> ConfirmEmailAsync(string token);
    }
}
