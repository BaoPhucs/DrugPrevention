using DrugPreventionAPI.Data;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class UserInquiryRepository
    {
        private readonly DataContext _ctx;
        public UserInquiryRepository(DataContext ctx) => _ctx = ctx;

        public async Task<UserInquiry> AddAsync(UserInquiry iq)
        {
            iq.CreatedDate = iq.LastUpdated = DateTime.UtcNow;
            _ctx.UserInquiries.Add(iq);
            await _ctx.SaveChangesAsync();
            return iq;
        }

        public async Task DeleteAsync(int id)
        {
            var iq = await _ctx.UserInquiries.FindAsync(id);
            if (iq != null)
            {
                _ctx.UserInquiries.Remove(iq);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserInquiry>> GetAllAsync()
            => await _ctx.UserInquiries
                         .Include(u => u.CreatedBy)
                         .ToListAsync();

        public async Task<UserInquiry> GetByIdAsync(int id)
            => await _ctx.UserInquiries
                         .Include(u => u.CreatedBy)
                         .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<IEnumerable<UserInquiry>> GetByUserAsync(int userId)
            => await _ctx.UserInquiries
                         .Where(u => u.CreatedById == userId)
                         .Include(u => u.CreatedBy)
                         .ToListAsync();

        public async Task<UserInquiry> UpdateAsync(UserInquiry iq)
        {
            iq.LastUpdated = DateTime.UtcNow;
            _ctx.UserInquiries.Update(iq);
            await _ctx.SaveChangesAsync();
            return iq;
        }
    }
}
