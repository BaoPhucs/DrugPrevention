using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IBlogPostRepo
    {
        Task<IEnumerable<BlogPost>> GetAllAsync();
        Task<BlogPost?> GetByIdAsync(int id);
        Task<BlogPost> AddAsync(BlogPost post, IEnumerable<int> tagIds);
        Task<BlogPost> UpdateAsync(UpdateBlogPostDTO dto);
        Task DeleteAsync(int id);
    }
}
