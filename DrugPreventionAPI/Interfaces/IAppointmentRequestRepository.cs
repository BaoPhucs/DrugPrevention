using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IAppointmentRequestRepository
    {
        Task<AppointmentRequest> GetByIdAsync(int id);
        Task<IEnumerable<AppointmentRequest>> GetByMemberAsync(int memberId);
        Task<IEnumerable<AppointmentRequest>> GetByConsultantAsync(int consultantId);
        Task<AppointmentRequest> CreateAsync(AppointmentRequest req);
        Task<bool> UpdateStatusAsync(int requestId, string status, string? cancelReason = null);
        Task<bool> DeleteAsync(int requestId); // cho phép member huỷ hẳn request trước khi consultant confirm
        Task<IEnumerable<AppointmentRequest>> GetAllAsync(); // (tuỳ nếu admin/manager cần xem tổng thể)

        Task<bool> MarkNoShowAsync(int requestId, string? reason = null);
        Task<IEnumerable<AppointmentRequest>> GetConfirmedAppointmentsForReminderAsync(TimeSpan leadTime);
    }
}
