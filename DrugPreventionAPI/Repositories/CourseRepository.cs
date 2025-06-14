using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DataContext _context;
        public CourseRepository(DataContext context)
        {
            _context = context;
        }

        
        public async Task<bool> CreateCourseAsync(Course course)
        {
            _context.Courses.Add(course); // Adds the course to the context
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return false; // Course not found
            }
            _context.Courses.Remove(course);
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.ToListAsync(); // Retrieves all courses from the database
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            return await _context.Courses.Where(c => c.Id == id).FirstOrDefaultAsync(); // Retrieves a course by its ID
        }

        public async Task<IEnumerable<Course>> GetCourseByLevelAsync(string level)
        {
            return await _context.Courses
                .Where(c => c.Level != null && c.Level.ToLower() == level.ToLower()).ToListAsync(); // Retrieves a course by its level
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(string category)
        {
            return await _context.Courses
                .Where(c => c.Category != null && c.Category.ToLower() == category.ToLower()).ToListAsync(); // Retrieves courses by category
        }

        public async Task<bool> ApproveAsync(int courseId)
        {
            var course = _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return false; // Course not found
            }
            course.Result.Status = "Approved"; // Set the course status to Approved
            _context.Courses.Update(course.Result); // Update the course in the context
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }


        public async Task<bool> RejectAsync(int courseId, string comments)
        {
            var course = _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return false; // Course not found
            }
            course.Result.Status = "Rejected"; // Set the course status to Rejected
            course.Result.ReviewComments = comments; // Store rejection comments
            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }
        

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            var existingCourse = await _context.Courses.FindAsync(course.Id);
            if (existingCourse == null)
            {
                return false; // Course not found
            }
            existingCourse.Title = course.Title;
            existingCourse.Image = course.Image;
            existingCourse.Description = course.Description;
            existingCourse.Content = course.Content;
            existingCourse.Category = course.Category;
            existingCourse.Level = course.Level;
            existingCourse.Duration = course.Duration;
            existingCourse.PassingScore = course.PassingScore;
            existingCourse.Status = course.Status;

            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected

        }
    }
}
