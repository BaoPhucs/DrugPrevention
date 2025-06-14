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

        //CRUD
        Task<bool> CreateCourseAsync(Course course); // Adds a new course
        Task<bool> UpdateCourseAsync(Course course); // Updates an existing course
        Task<bool> DeleteCourseAsync(int id); // Deletes a course by its ID

        //Content‐workflow
        Task<bool> ApproveAsync(int courseId);                   // Manager duyệt
        Task<bool> RejectAsync(int courseId, string comments);


    }
}
