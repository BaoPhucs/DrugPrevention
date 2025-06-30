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
            course.Result.WorkflowState = "Approved"; // Update the workflow state to Approved
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
            course.Result.WorkflowState = "Draft"; // Update the workflow state to Rejected
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

        public async Task<IEnumerable<Course>> GetCoursesByCreatedByIdAsync(int createById)
        {
            return await _context.Courses
                .Where(c => c.CreatedById == createById)
                .ToListAsync(); // Retrieves courses created by a specific user
        }

        public async Task<IEnumerable<Course>> GetCoursesByStatusAsync(string status)
        {
            return await _context.Courses
                .Where(c => c.Status != null && c.Status.ToLower() == status.ToLower())
                .ToListAsync(); // Retrieves courses by their status
        }

        public async Task<bool> SubmitToStaffAsync(int courseId, int userId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null || course.WorkflowState != "Draft")
                return false;

            course.WorkflowState = "SubmittedToStaff";
            course.UpdateById = userId;
            course.UpdateDate = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SubmitToManagerAsync(int courseId, int userId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            course.WorkflowState = "SubmittedToManager";
            course.UpdateById = userId;
            course.UpdateDate = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> PublicCourse(int couseId, int userId)
        {
            var course = await _context.Courses.FindAsync(couseId);
            if (course == null || course.WorkflowState != "Approved" || course.Status != "Approved")
                return false;
            course.Status = "Published";
            course.UpdateById = userId;
            course.UpdateDate = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SchedulePublishAsync(int courseId, DateTime publishAt)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return false;
            // Chỉ cho phép khi đã được Manager approve
            if (course.WorkflowState != "Approved") return false;

            course.PublishAt = publishAt;
            //course.WorkflowState = "Published"; // đánh dấu đã lên lịch
            _context.Courses.Update(course);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> PublishIfDueAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return false;
            // chỉ publish nếu đã scheduled và đến giờ
            if (course.WorkflowState != "Approved" || course.PublishAt > DateTime.UtcNow || course.Status != "Approved")
                return false;
            course.Status = "Published";
            _context.Courses.Update(course);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> PublishAllDueAsync()
        {
            var now = DateTime.UtcNow;
            var due = await _context.Courses
                .Where(c => c.WorkflowState == "Approved" && c.PublishAt <= now && c.Status == "Approved")
                .ToListAsync();
            foreach (var c in due)
                c.Status = "Published";
            await _context.SaveChangesAsync();
            return due.Count;
        }
    }
}
