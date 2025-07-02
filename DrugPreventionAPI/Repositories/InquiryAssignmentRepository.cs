using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class InquiryAssignmentRepository : IInquiryAssignmentRepository
    {
        private readonly DataContext _ctx;
        public InquiryAssignmentRepository(DataContext ctx) => _ctx = ctx;

        public async Task<InquiryAssignment> AddAsync(InquiryAssignment a)
        {
            _ctx.InquiryAssignments.Add(a);
            await _ctx.SaveChangesAsync();
            return a;
        }

        public async Task<IEnumerable<InquiryAssignment>> GetByInquiryIdAsync(int inquiryId)
            => await _ctx.InquiryAssignments
                         .Include(x => x.AssignedBy)
                         .Include(x => x.AssignedTo)
                         .Where(x => x.InquiryId == inquiryId)
                         .ToListAsync();

        public async Task<InquiryAssignment> GetByIdAsync(int id)
            => await _ctx.InquiryAssignments
                         .Include(x => x.AssignedBy)
                         .Include(x => x.AssignedTo)
                         .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<InquiryAssignment> UpdateAsync(InquiryAssignment a)
        {
            _ctx.InquiryAssignments.Update(a);
            await _ctx.SaveChangesAsync();
            return a;
        }

        public async Task<IEnumerable<InquiryAssignment>> GetByAssignedToIdIdAsync(int assignedToId)
        {
            return await _ctx.InquiryAssignments
                .Include(x => x.AssignedTo)
                .Include(x => x.AssignedBy)
                .Where(x => x.AssignedToId == assignedToId)
                .ToListAsync();
        }
    }
}
