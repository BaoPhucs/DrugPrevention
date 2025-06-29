using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IConsultationNoteRepository
    {
        Task<IEnumerable<ConsultationNote>> GetByAppointmentAsync(int appointmentId);
        Task<ConsultationNote> AddAsync(ConsultationNote note);
        Task<bool> UpdateNoteAssync(int id, string notes);
    }
}
