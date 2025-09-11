using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class CommunicationActivityRepository : ICommunicationActivityRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<CommunicationActivityRepository> _logger;
        public CommunicationActivityRepository(DataContext context, ILogger<CommunicationActivityRepository> logger)
        {
            _context = context;
            _logger = logger;
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

        public async Task<CommunicationActivity?> UpdateAsync(int id, CommunicationActivity updated, int userId)
        {
            var existing = await _context.CommunicationActivities.FindAsync(id);
            if (existing == null) return null;

            // Kiểm tra quyền sở hữu
            if (existing.CreatedById.HasValue && existing.CreatedById != userId)
            {
                return null; // Trả về null để controller xử lý NotFound hoặc Forbid
            }

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
            existing.Status = "Pending"; // Đảm bảo Status không bị ghi đè không mong muốn

            await _context.SaveChangesAsync();
            return existing;
        }

        //public async Task<CommunicationActivity?> UpdateAsync(int id, CommunicationActivity updated)
        //{
        //    var existing = await _context.CommunicationActivities.FindAsync(id);
        //    if (existing == null) return null;

        //    // Chỉ cập nhật các thuộc tính khi giá trị được cung cấp và không phải giá trị mặc định/null
        //    if (updated.Title != null && !string.IsNullOrWhiteSpace(updated.Title))
        //    {
        //        existing.Title = updated.Title;
        //    }

        //    if (updated.Description != null && !string.IsNullOrWhiteSpace(updated.Description))
        //    {
        //        existing.Description = updated.Description;
        //    }

        //    if (updated.EventDate.HasValue)
        //    {
        //        existing.EventDate = updated.EventDate;
        //    }

        //    if (updated.Location != null && !string.IsNullOrWhiteSpace(updated.Location))
        //    {
        //        existing.Location = updated.Location;
        //    }

        //    if (updated.Capacity.HasValue && updated.Capacity > 0) // Đảm bảo Capacity hợp lệ
        //    {
        //        existing.Capacity = updated.Capacity;
        //    }

        //    // Cập nhật Status chỉ khi được cung cấp và không rỗng
        //    if (updated.Status != null && !string.IsNullOrWhiteSpace(updated.Status))
        //    {
        //        existing.Status = updated.Status;
        //    }

        //    // Cập nhật ngày cập nhật nếu có thay đổi
        //    existing.CreatedDate = existing.CreatedDate; // Giữ nguyên CreatedDate
        //    existing.Status = existing.Status; // Đảm bảo Status không bị ghi đè không mong muốn

        //    await _context.SaveChangesAsync();
        //    return existing;
        //}

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = await _context.CommunicationActivities
                    .Include(a => a.ActivityParticipations)
                    .Include(a => a.Comments)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (entity == null) return false;

                // Xóa tất cả ActivityParticipations liên quan
                if (entity.ActivityParticipations?.Any() == true)
                {
                    _context.ActivityParticipations.RemoveRange(entity.ActivityParticipations);
                }

                // Xóa tất cả Comments liên quan
                if (entity.Comments?.Any() == true)
                {
                    _context.Comments.RemoveRange(entity.Comments);
                }

                // Xóa CommunicationActivity
                _context.CommunicationActivities.Remove(entity);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false; // Trả về false nếu có lỗi
            }
        }

        public async Task<CommunicationActivity?> SubmitForApprovalAsync(int id)
        {
            var activity = await _context.CommunicationActivities.FindAsync(id);
            if (activity.Status != "Pending" && activity.Status != "Rejected") return null;

            activity.Status = "Submitted";
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<CommunicationActivity?> ApproveAsync(int id)
        {
            var activity = await _context.CommunicationActivities.FindAsync(id);
            if (activity == null || activity.Status != "Submitted") return null;

            activity.Status = "Approved";
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<CommunicationActivity?> RejectAsync(int id, string? reviewComments)
        {
            var activity = await _context.CommunicationActivities.FindAsync(id);
            if (activity == null || activity.Status != "Submitted") return null;

            activity.Status = "Rejected";
            activity.ReviewComments = reviewComments; // Ghi lý do reject
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<CommunicationActivity?> PublishAsync(int id)
        {
            var activity = await _context.CommunicationActivities.FindAsync(id);
            if (activity == null || activity.Status != "Approved") return null;

            activity.Status = "Published";
            await _context.SaveChangesAsync();
            return activity;
        }

        //public async Task<(int CancelledCount, Dictionary<int, (string Title, string[] Emails)> Details)> CheckAndCancelAllUnderCapacityAsync()
        //{
        //    var activities = await _context.CommunicationActivities
        //        .Include(a => a.ActivityParticipations)
        //        .ToListAsync();

        //    var cancelledCount = 0;
        //    var details = new Dictionary<int, (string Title, string[] Emails)>();

        //    foreach (var activity in activities)
        //    {
        //        if (activity.Status == "Published" &&
        //            activity.RegistrationDeadline.HasValue && DateTime.UtcNow > activity.RegistrationDeadline)
        //        {
        //            int registeredCount = activity.ActivityParticipations.Count(p => p.Status == "Registered");
        //            if (activity.Capacity.HasValue && registeredCount < (activity.Capacity.Value * 0.6))
        //            {
        //                activity.Status = "Cancelled";
        //                cancelledCount++;

        //                var participantEmails = await _context.ActivityParticipations
        //                    .Where(p => p.ActivityId == activity.Id && p.Status == "Registered")
        //                    .Include(p => p.Member)
        //                    .Select(p => p.Member.Email)
        //                    .ToListAsync();

        //                details[activity.Id] = (activity.Title ?? "Unknown Event", participantEmails?.ToArray() ?? Array.Empty<string>());
        //            }
        //        }
        //    }

        //    if (cancelledCount > 0)
        //    {
        //        await _context.SaveChangesAsync();
        //    }

        //    return (cancelledCount, details);
        //}
        public async Task<(int CancelledCount, Dictionary<int, (string Title, string[] Emails)> Details)> CheckAndCancelAllUnderCapacityAsync()
        {
            var activities = await _context.CommunicationActivities
                .Include(a => a.ActivityParticipations)
                .ToListAsync();

            var cancelledCount = 0;
            var details = new Dictionary<int, (string Title, string[] Emails)>();

            foreach (var activity in activities)
            {
                _logger.LogInformation($"Checking activity {activity.Id}: Status={activity.Status}, Deadline={activity.RegistrationDeadline}, Capacity={activity.Capacity}, Registered={activity.ActivityParticipations.Count(p => p.Status == "Registered")}");

                if (activity.Status == "Published" &&
                    activity.RegistrationDeadline.HasValue && DateTime.UtcNow > activity.RegistrationDeadline.Value.ToUniversalTime())
                {
                    int registeredCount = activity.ActivityParticipations.Count(p => p.Status == "Registered");
                    _logger.LogInformation($"Activity {activity.Id} - RegisteredCount={registeredCount}, Threshold={(activity.Capacity.HasValue ? activity.Capacity.Value * 0.6 : 0)}");

                    if (activity.Capacity.HasValue && registeredCount < (activity.Capacity.Value * 0.6))
                    {
                        activity.Status = "Cancelled";
                        cancelledCount++;

                        var participantEmails = await _context.ActivityParticipations
                            .Where(p => p.ActivityId == activity.Id && p.Status == "Registered")
                            .Include(p => p.Member)
                            .Select(p => p.Member.Email)
                            .Where(e => !string.IsNullOrEmpty(e)) // Loại bỏ email null
                            .ToListAsync();

                        details[activity.Id] = (activity.Title ?? "Unknown Event", participantEmails?.ToArray() ?? Array.Empty<string>());
                        _logger.LogInformation($"Activity {activity.Id} marked for cancellation. Emails: {string.Join(",", participantEmails ?? new List<string>())}");
                    }
                }
            }

            if (cancelledCount > 0)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation($"Successfully cancelled {cancelledCount} activities.");
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Database error when saving cancelled activities: {ex.InnerException?.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Unexpected error when saving cancelled activities.");
                    throw;
                }
            }

            return (cancelledCount, details);
        }
        
    }
}
