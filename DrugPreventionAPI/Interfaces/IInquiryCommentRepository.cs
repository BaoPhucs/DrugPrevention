using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IInquiryCommentRepository
    {
        Task<InquiryComment> GetByIdAsync(int id);
        Task<IEnumerable<InquiryComment>> GetByInquiryAsync(int inquiryId);
        Task<InquiryComment> AddAsync(InquiryComment comment);
        Task<InquiryComment> UpdateAsync(InquiryComment comment);
    }
}
