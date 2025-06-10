using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> LoginAsync(string email, string password);
        Task<User> AuthenticateGoogleAsync(string googleToken); // Authenticates a user using Google OAuth
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword, string confirmPassword); // Changes the password for a user
        Task<bool> ResetPasswordAsync(string email); // Resets the password for a user by email

    }
}
