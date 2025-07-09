using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ICommunicationActivityRepository
    {
        Task<IEnumerable<CommunicationActivity>> GetAllAsync();
        Task<CommunicationActivity?> GetByIdAsync(int id);
        Task<CommunicationActivity> CreateAsync(CommunicationActivity activity);
        Task<CommunicationActivity?> UpdateAsync(int id, CommunicationActivity updated);
        Task<bool> DeleteAsync(int id);
        Task<CommunicationActivity?> SubmitForApprovalAsync(int id); // Gửi phê duyệt
        Task<CommunicationActivity?> ApproveAsync(int id); // Phê duyệt
        Task<CommunicationActivity?> RejectAsync(int id, string? reviewComments); // Từ chối
        Task<CommunicationActivity?> PublishAsync(int id); // Đăng bài
    }
}
