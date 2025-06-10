using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IUserManagementRepository
    {
        
        
        
        Task<bool> RegisterAsync(User user);
        Task<bool> UpdateAsync(User user);
        
        
        
        
        
        
        
    }
}
