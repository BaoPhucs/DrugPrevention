using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IConfiguration _configuration;

        private readonly DataContext _context;
        public AdminRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public Task<bool> DeleteAsync(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return Task.FromResult(false); // User not found
            }
            _context.Users.Remove(user);
            return Task.FromResult(_context.SaveChanges() > 0); // Returns true if at least one row was affected
        }

        public async Task<bool> AssignRoleAsync(int id, string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false; // User not found
            }
            user.Role = role;
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users.Where(u => u.Role == role).ToListAsync(); // Retrieves users by their role
        }

        public async Task<bool> UserExistAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
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

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync(); // Returns the total number of users in the system
        }

        public async Task<bool> UpdateUserByAdminAsync(User user)
        {
            var ex = await _context.Users
                           .FirstOrDefaultAsync(u => u.Id == user.Id);
            if (ex == null)
                return false;

            ex.Name = user.Name;
            ex.Role = user.Role;
            ex.AgeGroup = user.AgeGroup;
            ex.ProfileData = user.ProfileData;
            // không cập nhật Password, EmailVerified,…

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ForceResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            user.Password = newPassword;  // hãy hash mật khẩu
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
