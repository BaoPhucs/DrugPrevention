using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DataContext _context;
        public NotificationRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Notification> CreateAsync(Notification note)
        {
            _context.Notifications.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<IEnumerable<Notification>> GetByUserAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SendDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
