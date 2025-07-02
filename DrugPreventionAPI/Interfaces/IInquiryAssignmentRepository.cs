using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IInquiryAssignmentRepository
    {
        Task<InquiryAssignment> GetByIdAsync(int id);
        Task<IEnumerable<InquiryAssignment>> GetByInquiryIdAsync(int inquiryId);
        Task<IEnumerable<InquiryAssignment>> GetByAssignedToIdIdAsync(int assignedToId);
        Task<InquiryAssignment> AddAsync(InquiryAssignment assignment);
        Task<InquiryAssignment> UpdateAsync(InquiryAssignment assignment);
    }
}
