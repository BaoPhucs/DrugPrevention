using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IAdminRepository
    {
        Task<bool> DeleteAsync(int id);
        Task<bool> AssignRoleAsync(int id, string role); // Assigns a role to a user by their ID
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role); // Retrieves users by their role
        Task<int> GetUserCountAsync();// Returns the total number of users in the system
        Task<bool> UserExistAsync(string email); // Checks if a user exists by email
        Task<User> GetByIdAsync(int id);
        Task<User> GetProfileAsync(string email);

        Task<bool> UpdateUserByAdminAsync(User user);
        Task<bool> ForceResetPasswordAsync(int userId, string newPassword);

        Task<int> CountUser();
        Task<int> CountCourseEnrollment();
        Task<int> CountCourseEnrollmentByCourseId(int courseId);
        Task<int> CountSurveySubmission(int surveyId);
        Task<int> CountPassedCourse();
    }
}
