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

            // Chỉ cập nhật các thuộc tính khi giá trị được cung cấp và không phải giá trị mặc định/null
            if (updated.Title != null && !string.IsNullOrWhiteSpace(updated.Title))
            {
                existing.Title = updated.Title;
            }

            if (updated.Description != null && !string.IsNullOrWhiteSpace(updated.Description))
            {
                existing.Description = updated.Description;
            }

            if (updated.EventDate.HasValue)
            {
                existing.EventDate = updated.EventDate;
            }

            if (updated.Location != null && !string.IsNullOrWhiteSpace(updated.Location))
            {
                existing.Location = updated.Location;
            }

            if (updated.Capacity.HasValue && updated.Capacity > 0) // Đảm bảo Capacity hợp lệ
            {
                existing.Capacity = updated.Capacity;
            }

            // Cập nhật Status chỉ khi được cung cấp và không rỗng
            if (updated.Status != null && !string.IsNullOrWhiteSpace(updated.Status))
            {
                existing.Status = updated.Status;
            }

            // Cập nhật ngày cập nhật nếu có thay đổi
            existing.CreatedDate = existing.CreatedDate; // Giữ nguyên CreatedDate
            existing.Status = existing.Status; // Đảm bảo Status không bị ghi đè không mong muốn

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
