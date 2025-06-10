using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<User> LoginAsync(string email, string password);
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetProfileAsync(string email);
        Task<bool> RegisterAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id); 
        Task<bool> AssignRoleAsync(int id, string role); // Assigns a role to a user by their ID
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role); // Retrieves users by their role
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword); // Changes the password for a user
        Task<bool> ResetPasswordAsync(string email); // Resets the password for a user by email
        Task<User> AuthenticateGoogleAsync(string googleToken); // Authenticates a user using Google OAuth
        Task<bool> UserExistAsync(string email); // Checks if a user exists by email
        Task<int> GetUserCountAsync();// Returns the total number of users in the system
    }
}
