using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IUserInquiryRepository
    {
        Task<UserInquiry> GetByIdAsync(int id);
        Task<IEnumerable<UserInquiry>> GetAllAsync();
        Task<IEnumerable<UserInquiry>> GetByUserAsync(int userId);
        Task<UserInquiry> AddAsync(UserInquiry inquiry);
        Task<UserInquiry> UpdateAsync(UserInquiry inquiry);
        Task DeleteAsync(int id);
    }
}
