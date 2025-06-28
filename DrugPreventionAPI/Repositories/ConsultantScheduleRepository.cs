using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class ConsultantScheduleRepository : IConsultantScheduleRepository
    {
        private readonly DataContext _context;
        public ConsultantScheduleRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ConsultantSchedule> AddAsync(ConsultantSchedule slot)
        {
            await _context.ConsultantSchedules.AddAsync(slot);
            await _context.SaveChangesAsync();
            return slot;
        }

        public async Task<bool> DeleteAsync(int slotId)
        {
            var slot = await _context.ConsultantSchedules.FindAsync(slotId);
            if (slot == null) return false;
            _context.ConsultantSchedules.Remove(slot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ConsultantSchedule>> GetAvailableByConsultantAsync(int consultantId)
        {
            return await _context.ConsultantSchedules
                     .Where(s => s.ConsultantId == consultantId && s.IsAvailable == true)
                     .ToListAsync();
        }

        public async Task<IEnumerable<ConsultantSchedule>> GetByIsAvailabilityAsync(bool isAvailable)
        {
            return await _context.ConsultantSchedules
                     .Where(s => s.IsAvailable == isAvailable)
                     .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetConsultant()
        {
            return await _context.Users
                     .Where(u => u.Role == "Consultant")
                     .ToListAsync();
        }

        public async Task<ConsultantSchedule> GetScheduleById(int scheduleId)
        {
            return await _context.ConsultantSchedules
                .Where(s => s.Id == scheduleId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(ConsultantSchedule slot)
        {
            var existing = await _context.ConsultantSchedules.FindAsync(slot.Id);
            if (existing == null) return false;
            existing.ScheduleDate = slot.ScheduleDate;
            existing.StartTime = slot.StartTime;
            existing.EndTime = slot.EndTime;
            existing.IsAvailable = slot.IsAvailable;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
