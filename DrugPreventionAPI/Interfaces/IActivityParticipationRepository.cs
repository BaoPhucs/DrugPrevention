using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IActivityParticipationRepository
    {
        Task<IEnumerable<ActivityParticipation>> GetByActivityIdAsync(int activityId);
        Task<ActivityParticipation?> GetByMemberAndActivityAsync(int memberId, int activityId);
        Task<ActivityParticipation> RegisterAsync(ActivityParticipation participation);
        Task<bool> DeleteAsync(int id);
        Task<ActivityParticipation?> UpdateStatusAsync(int id, string status);
        Task<IEnumerable<ActivityParticipation>> GetAllAsync();

        Task<ActivityParticipation?> CancelParticipationAsync(int memberId, int activityId);

    }
}
