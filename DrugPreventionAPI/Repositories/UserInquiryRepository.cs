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
        {
            return await _ctx.UserInquiries.ToListAsync();
        }

        public async Task<UserInquiry> GetByIdAsync(int id)
        {
            return await _ctx.UserInquiries.FindAsync(id);
        }

        public async Task<IEnumerable<UserInquiry>> GetByUserAsync(int userId)
        {
            return await _ctx.UserInquiries.Where(i => i.Id == userId).ToListAsync();
        }

        public async Task<UserInquiry> UpdateAsync(UserInquiry inquiry)
        {
            _ctx.UserInquiries.Update(inquiry);
            await _ctx.SaveChangesAsync();
            return inquiry;
        }
    }
}
