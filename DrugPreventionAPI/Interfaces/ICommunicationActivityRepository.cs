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

    }
}
