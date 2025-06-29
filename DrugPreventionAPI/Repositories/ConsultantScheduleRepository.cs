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

        public async Task<int> DisableSlotsExpiringWithinAsync(TimeSpan within)
        {
            var now = DateTime.UtcNow;
            var cutoff = now.Add(within);

            // tách DateOnly/TimeOnly để so sánh
            var nowDate = DateOnly.FromDateTime(now);
            var nowTime = TimeOnly.FromDateTime(now);
            var cutDate = DateOnly.FromDateTime(cutoff);
            var cutTime = TimeOnly.FromDateTime(cutoff);

            var toDisable = await _context.ConsultantSchedules
                .Where(s => s.IsAvailable == true
                         && s.ScheduleDate.HasValue
                         && s.StartTime.HasValue

                         // phải > now
                         && (
                               s.ScheduleDate > nowDate
                            || (s.ScheduleDate == nowDate && s.StartTime > nowTime)
                            )

                         // và ≤ cutoff
                         && (
                               s.ScheduleDate < cutDate
                            || (s.ScheduleDate == cutDate && s.StartTime <= cutTime)
                            )
                      )
                .ToListAsync();

            foreach (var slot in toDisable)
            {
                slot.IsAvailable = false;
            }

            await _context.SaveChangesAsync();
            return toDisable.Count;
        }
    }
}
