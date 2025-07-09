using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IBlogPostRepo
    {
        Task<IEnumerable<BlogPost>> GetAllAsync();
        Task<BlogPost?> GetByIdAsync(int id);
        Task<IEnumerable<BlogPost>> GetByTagId(int tagId);
        Task<BlogPost> AddAsync(BlogPost post, IEnumerable<int> tagIds);
        Task<BlogPost> UpdateAsync(int id, UpdateBlogPostDTO dto);
        Task DeleteAsync(int id);
        Task<BlogPost?> SubmitForApprovalAsync(int id); // Gửi phê duyệt
        Task<BlogPost?> ApproveAsync(int id); // Phê duyệt
        Task<BlogPost?> RejectAsync(int id, string? reviewComments); // Từ chối với lý do
        Task<BlogPost?> PublishAsync(int id); // Đăng bài
    }
}
