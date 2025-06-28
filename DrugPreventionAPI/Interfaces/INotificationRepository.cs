using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification> CreateAsync(Notification note);
        Task<IEnumerable<Notification>> GetByUserAsync(int userId);
    }
}
