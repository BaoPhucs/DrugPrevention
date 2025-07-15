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

        public async Task<bool> CancelAsync(int requestId, string? reason = null)
        {
            var req = await _context.AppointmentRequests
                                .FirstOrDefaultAsync(r => r.Id == requestId);
            if (req == null)
                return false;

            // Cập nhật lý do hủy trước khi xóa
            req.Status = "Cancelled";
            req.CancelReason = reason;
            req.CancelledDate = DateTime.UtcNow; // Cập nhật thời gian hủy (nếu cần)

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

        public async Task<bool> MarkNoShowAsync(int requestId, string? reason = null)
        {
            var req = await _context.AppointmentRequests
                        .Include(r => r.Schedule)
                        .Include(r => r.Member)
                        .FirstOrDefaultAsync(r => r.Id == requestId);
            if (req == null) return false;

            req.Status = "NoShow";
            req.CancelReason = reason;
            req.NoShowDate = DateTime.UtcNow;
            req.NoShowCount += 1;

            // Tăng tổng NoShow trên user (nếu dùng)
            if (req.Member != null)
                req.Member.NoShowTotal += 1;

            // Đếm NoShow trong 30 ngày qua
            var cutoff = DateTime.UtcNow.AddDays(-30);
            var recentNoShows = await _context.AppointmentRequests
                .CountAsync(r =>
                    r.MemberId == req.MemberId
                    && r.Status == "NoShow"
                    && r.NoShowDate >= cutoff);

            // Nếu >=3 lần, khoá account 7 ngày
            if (recentNoShows >= 3 && req.Member != null)
            {
                //req.Member.LockoutEnd = DateTime.UtcNow.AddDays(7);
                req.Member.LockoutEnd = DateTime.UtcNow.AddMinutes(1); // Giả lập khoá 1 phút để test

            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AppointmentRequest>> GetConfirmedAppointmentsForReminderAsync(TimeSpan leadTime)
        {
            var now = DateTime.UtcNow;
            var target = now.Add(leadTime);

            // Mở rộng ±5 phút để tránh bỏ sót
            var windowStart = target.AddMinutes(-5);
            var windowEnd = target.AddMinutes(+5);

            // Danh sách userId đã từng nhận reminder
            var alreadySent = await _context.Notifications
                .Where(n => n.Type == "AppointmentReminder")
                .Select(n => n.UserId)
                .ToListAsync();

            // 1) Lọc sơ bộ theo ngày
            var dateMin = DateOnly.FromDateTime(windowStart);
            var dateMax = DateOnly.FromDateTime(windowEnd);

            var prelim = await _context.AppointmentRequests
                .Include(a => a.Member)
                .Include(a => a.Schedule)
                .Where(a =>
                    a.Status == "Confirmed"
                    && a.Schedule != null
                    && a.Schedule.ScheduleDate.HasValue
                    && a.Schedule.ScheduleDate.Value >= dateMin
                    && a.Schedule.ScheduleDate.Value <= dateMax
                    && a.MemberId.HasValue
                    && !alreadySent.Contains(a.MemberId.Value)
                )
                .ToListAsync();

            // 2) Chuyển sang DateTime và lọc tiếp theo giờ
            var result = new List<AppointmentRequest>();
            foreach (var appt in prelim)
            {
                var sd = appt.Schedule.ScheduleDate!.Value;
                var st = appt.Schedule.StartTime!.Value;
                // ghép DateOnly + TimeOnly => DateTime
                var dt = sd.ToDateTime(st);

                if (dt >= windowStart && dt <= windowEnd)
                {
                    result.Add(appt);
                }
            }

            return result;
        }
    }
}
