using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class ConsultationNoteRepository : IConsultationNoteRepository
    {
        private readonly DataContext _context;
        public ConsultationNoteRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ConsultationNote> AddAsync(ConsultationNote note)
        {
            _context.ConsultationNotes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<IEnumerable<ConsultationNote>> GetByAppointmentAsync(int appointmentId)
        {
            return await _context.ConsultationNotes
                     .Where(n => n.AppointmentId == appointmentId)
                     .ToListAsync();
        }

        public async Task<bool> UpdateNoteAssync(int id, string notes)
        {
            var note = await _context.ConsultationNotes
                                     .FirstOrDefaultAsync(n => n.Id == id);
            if (note == null) return false;

            note.Notes = notes;
            // nếu cần cập nhật thêm timestamp:
            // note.UpdatedDate = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

    }
}
