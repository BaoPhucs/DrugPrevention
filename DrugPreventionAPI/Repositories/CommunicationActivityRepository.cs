using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class CommunicationActivityRepository : ICommunicationActivityRepository
    {
        private readonly DataContext _context;

        public CommunicationActivityRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommunicationActivity>> GetAllAsync()
        {
            return await _context.CommunicationActivities.ToListAsync();
        }

        public async Task<CommunicationActivity?> GetByIdAsync(int id)
        {
            return await _context.CommunicationActivities.FindAsync(id);
        }

        public async Task<CommunicationActivity> CreateAsync(CommunicationActivity activity)
        {
            _context.CommunicationActivities.Add(activity);
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<CommunicationActivity?> UpdateAsync(int id, CommunicationActivity updated)
        {
            var existing = await _context.CommunicationActivities.FindAsync(id);
            if (existing == null) return null;

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.EventDate = updated.EventDate;
            existing.Location = updated.Location;
            existing.Capacity = updated.Capacity;
            existing.Status = updated.Status;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.CommunicationActivities.FindAsync(id);
            if (entity == null) return false;

            _context.CommunicationActivities.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
