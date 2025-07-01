using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class UserInquiryRepository : IUserInquiryRepository
    {
        private readonly DataContext _ctx;

        public UserInquiryRepository(DataContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<UserInquiry> AddAsync(UserInquiry inquiry)
        {
            await _ctx.UserInquiries.AddAsync(inquiry);
            await _ctx.SaveChangesAsync();
            return inquiry;
        }

        public async Task DeleteAsync(int id)
        {
            var inquiry = await _ctx.UserInquiries.FindAsync(id);
            if (inquiry != null)
            {
                _ctx.UserInquiries.Remove(inquiry);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserInquiry>> GetAllAsync()
         => await _ctx.UserInquiries
        .Include(ui => ui.CreatedBy)
        .ToListAsync();

        public async Task<UserInquiry> GetByIdAsync(int id)
        {
            return await _ctx.UserInquiries
                  .Include(ui => ui.CreatedBy)           // ← load the User row
                  .Include(ui => ui.InquiryComments)
                  .Include(ui => ui.InquiryAssignments)
                  .FirstOrDefaultAsync(ui => ui.Id == id);
        }

        public async Task<IEnumerable<UserInquiry>> GetByUserAsync(int userId)
        =>
            await _ctx.UserInquiries
                    .Where(ui => ui.CreatedById == userId) // Lọc theo CreatedById
                    .Include(ui => ui.InquiryAssignments) // Bao gồm navigation properties nếu cần
                    .Include(ui => ui.InquiryComments)
                    .ToListAsync();


        public async Task<UserInquiry> UpdateAsync(UserInquiry inquiry)
        {
            _ctx.UserInquiries.Update(inquiry);
            await _ctx.SaveChangesAsync();
            return inquiry;
        }
    }
}
