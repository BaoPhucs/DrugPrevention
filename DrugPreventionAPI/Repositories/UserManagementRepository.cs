using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly IConfiguration _configuration;

        private readonly DataContext _context;
        public UserManagementRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetCurrentUserAsync(int currentUserId)
        {
            return await GetByIdAsync(currentUserId);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

       
    }
}
