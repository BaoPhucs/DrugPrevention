using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ICourseRepository
    {
        //Catalog
        Task<IEnumerable<Course>> GetAllCoursesAsync(); // Retrieves all courses
        Task<Course> GetCourseByIdAsync(int id); // Retrieves a course by its ID
        Task<IEnumerable<Course>> GetCoursesByCategoryAsync(string category); // Retrieves courses by category
        Task<IEnumerable<Course>> GetCourseByLevelAsync(string level); // Retrieves a course by its title
        Task<IEnumerable<Course>> GetCoursesByCreatedByIdAsync(int createById); // Retrieves courses by title
        Task<IEnumerable<Course>> GetCoursesByStatusAsync(string status); // Retrieves courses by title

        //CRUD
        Task<bool> CreateCourseAsync(Course course); // Adds a new course
        Task<bool> UpdateCourseAsync(Course course); // Updates an existing course
        Task<bool> DeleteCourseAsync(int id); // Deletes a course by its ID

        //Content‐workflow
        Task<bool> ApproveAsync(int courseId);                   // Manager duyệt
        Task<bool> RejectAsync(int courseId, string comments);
        Task<bool> PublicCourse(int couseId, int userId);
        Task<bool> SubmitToStaffAsync(int courseId, int userId);
        Task<bool> SubmitToManagerAsync(int courseId, int userId);

        Task<bool> SchedulePublishAsync(int courseId, DateTime publishAt);

        Task<bool> PublishIfDueAsync(int courseId);
        Task<int> PublishAllDueAsync();
    }
}
