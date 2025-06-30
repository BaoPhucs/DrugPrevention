using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IConsultantScheduleRepository
    {
        Task<IEnumerable<ConsultantSchedule>> GetAvailableByConsultantAsync(int consultantId);
        Task<IEnumerable<ConsultantSchedule>> GetByIsAvailabilityAsync(bool isAvailable);
        Task<IEnumerable<User>> GetConsultant();
        Task<ConsultantSchedule> GetScheduleById(int scheduleId);
        Task<ConsultantSchedule> AddAsync(ConsultantSchedule slot);
        Task<bool> UpdateAsync(ConsultantSchedule slot);
        Task<bool> DeleteAsync(int slotId);

        Task<int> DisableSlotsExpiringWithinAsync(TimeSpan within);
    }
}
