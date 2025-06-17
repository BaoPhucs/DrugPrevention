using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class InquiryCommentRepository : IInquiryCommentRepository
    {
        private readonly DataContext _ctx;
        public InquiryCommentRepository(DataContext ctx) => _ctx = ctx;

        public async Task<InquiryComment> AddAsync(InquiryComment comment)
        {
            _ctx.InquiryComments.Add(comment);
            await _ctx.SaveChangesAsync();
            return comment;
        }

        public async Task<IEnumerable<InquiryComment>> GetByInquiryAsync(int inquiryId)
            => await _ctx.InquiryComments
                         .Where(ic => ic.InquiryId == inquiryId)
                         .Include(ic => ic.CommentBy)
                         .ToListAsync();

        public async Task<InquiryComment> GetByIdAsync(int id)
            => await _ctx.InquiryComments
                         .Include(ic => ic.CommentBy)
                         .FirstOrDefaultAsync(ic => ic.Id == id);

        public async Task<InquiryComment> UpdateAsync(InquiryComment comment)
        {
            _ctx.InquiryComments.Update(comment);
            await _ctx.SaveChangesAsync();
            return comment;
        }
    }
}
