using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class AppointmentRequestRepository : IAppointmentRequestRepository
    {
        private readonly DataContext _context;
        public AppointmentRequestRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<AppointmentRequest> CreateAsync(AppointmentRequest req)
        {
            // 1) Thêm request
            _context.AppointmentRequests.Add(req);
            await _context.SaveChangesAsync(); // để đảm bảo req.Id và req.ScheduleId có giá trị

            // 2) Khóa slot đã được đặt
            var slot = await _context.ConsultantSchedules
                .FirstOrDefaultAsync(s => s.Id == req.ScheduleId);
            if (slot != null)
            {
                slot.IsAvailable = false;
                _context.ConsultantSchedules.Update(slot);
                await _context.SaveChangesAsync();
            }

            return req;
        }

        public async Task<bool> DeleteAsync(int requestId)
        {
            var req = await _context.AppointmentRequests
                                .FirstOrDefaultAsync(r => r.Id == requestId);
            if (req == null)
                return false;

            // Xóa request
            _context.AppointmentRequests.Remove(req);

            // Mở lại khung giờ này cho consultant
            var slot = await _context.ConsultantSchedules
                                 .FirstOrDefaultAsync(s => s.Id == req.ScheduleId);
            if (slot != null)
            {
                slot.IsAvailable = true;
                _context.ConsultantSchedules.Update(slot);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AppointmentRequest>> GetAllAsync()
        {
            return await _context.AppointmentRequests
                             .AsNoTracking()
                             .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentRequest>> GetByConsultantAsync(int consultantId)
        {
            return await _context.AppointmentRequests
                     .Where(r => r.ConsultantId == consultantId)
                     .ToListAsync();
        }

        public async Task<AppointmentRequest> GetByIdAsync(int id)
        {
            return await _context.AppointmentRequests.FindAsync(id)
           ?? throw new KeyNotFoundException("Request not found");
        }

        public async Task<IEnumerable<AppointmentRequest>> GetByMemberAsync(int memberId)
        {
            return await _context.AppointmentRequests
                     .Where(r => r.MemberId == memberId)
                     .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(int requestId, string status, string? cancelReason = null)
        {
            var r = await _context.AppointmentRequests.FindAsync(requestId);
            if (r == null) return false;
            r.Status = status;
            if (status == "Cancelled")
            {
                r.CancelledDate = DateTime.UtcNow;
                r.CancelReason = cancelReason;
            }
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
