using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        private readonly DataContext _context;
        public UserRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                var user = await GetByEmailAsync(email) ?? new User { Email = email, EmailVerified = true };
                if (user.Id == 0) await RegisterAsync(user);
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

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> RegisterAsync(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            return await _context.Users.ToListAsync(); // Returns all users in the system
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync(); // Returns a user by their ID
        }

        public async Task<User> GetProfileAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email); // Returns the profile of a user by their email
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AssignRoleAsync(int id, string role)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetPasswordAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UserExistAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);    
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync(); // Returns the total number of users in the system
        }
    }
}
