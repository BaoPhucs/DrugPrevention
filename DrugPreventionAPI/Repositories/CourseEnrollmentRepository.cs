using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class CourseEnrollmentRepository : ICourseEnrollmentRepository
    {
        private readonly DataContext _context;

        public CourseEnrollmentRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> CancelEnrollmentAsync(int courseId, int memberId)
        {
            var enrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.MemberId == memberId);
            if (enrollment == null)
            {
                return false; // Enrollment not found
            }

            _context.CourseEnrollments.Remove(enrollment);
            return await _context.SaveChangesAsync() > 0; // Returns true if the operation was successful
        }

        public async Task<bool> EnrollAsync(int courseId, int memberId)
        {
            var existingEnrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.MemberId == memberId);
            if (existingEnrollment != null)
            {
                return false; // Already enrolled
            }

            var enrollment = new CourseEnrollment
            {
                CourseId = courseId,
                MemberId = memberId,
                ParticipationDate = DateTime.UtcNow,
                Status = "Enrolled"
            };

            _context.CourseEnrollments.Add(enrollment);
            return await _context.SaveChangesAsync() > 0; // Returns true if the operation was successful
        }

        public async Task<int> GetEnrollmentCountAsync(int courseId)
        {
            return await _context.CourseEnrollments
                .CountAsync(e => e.CourseId == courseId && e.Status == "Enrolled");
        }

        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseAsync(int courseId)
        {
            return await _context.CourseEnrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Member) // Include member details if needed
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByUserAsync(int memberId)
        {
            return await _context.CourseEnrollments
                .Where(e => e.MemberId == memberId)
                .Include(e => e.Course) // Include course details if needed
                .ToListAsync();
        }

        public async Task<CourseEnrollment?> GetEnrollmentStatusAsync(int courseId, int memberId)
        {
            return await _context.CourseEnrollments.FirstOrDefaultAsync(e => e.CourseId == courseId && e.MemberId == memberId);
        }
    }
}
