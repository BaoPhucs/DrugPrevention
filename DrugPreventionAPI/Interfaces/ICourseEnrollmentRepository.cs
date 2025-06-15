using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ICourseEnrollmentRepository
    {
        Task<bool> EnrollAsync(int courseId, int memberId);

        Task<bool> CancelEnrollmentAsync(int courseId, int memberId);

        Task<CourseEnrollment?> GetEnrollmentStatusAsync(int courseId, int memberId);

        Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByUserAsync(int memberId);

        Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseAsync(int courseId);

        Task<int> GetEnrollmentCountAsync(int courseId);
    }
}
