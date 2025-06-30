using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class ActivityParticipationRepository : IActivityParticipationRepository
    {
        private readonly DataContext _context;

        public ActivityParticipationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ActivityParticipation?> CancelParticipationAsync(int memberId, int activityId)
        {
            var participation = await _context.ActivityParticipations
                .FirstOrDefaultAsync(p => p.ActivityId == activityId && p.MemberId == memberId);

            if (participation == null || participation.Status == "Cancelled")
                return null;

            participation.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return participation;
        }

        public async Task<IEnumerable<ActivityParticipation>> GetAllAsync()
        {
            return await _context.ActivityParticipations
                .Include(p => p.Member)
                .Include(p => p.Activity)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityParticipation>> GetByActivityIdAsync(int activityId)
        {
            return await _context.ActivityParticipations
                .Where(p => p.ActivityId == activityId)
                .ToListAsync();
        }

        public async Task<ActivityParticipation?> GetByMemberAndActivityAsync(int memberId, int activityId)
        {
            return await _context.ActivityParticipations
                .FirstOrDefaultAsync(p => p.ActivityId == activityId && p.MemberId == memberId);
        }

        public async Task<ActivityParticipation> RegisterAsync(ActivityParticipation participation)
        {
            _context.ActivityParticipations.Add(participation);
            await _context.SaveChangesAsync();
            return participation;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ActivityParticipations.FindAsync(id);
            if (entity == null) return false;

            _context.ActivityParticipations.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ActivityParticipation?> UpdateStatusAsync(int id, string status)
        {
            var participation = await _context.ActivityParticipations.FindAsync(id);
            if (participation == null) return null;

            participation.Status = status;
            await _context.SaveChangesAsync();
            return participation;
        }

        

    }
}
