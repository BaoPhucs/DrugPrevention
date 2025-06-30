using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ITagRepo
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(int id);
        Task<Tag> AddAsync(Tag tag);
        Task<Tag> UpdateAsync(Tag tag);
        Task DeleteAsync(int id);
    }
}
