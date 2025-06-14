using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class CourseMaterialRepository : ICourseMaterialRepository
    {
        private readonly DataContext _context;
        public CourseMaterialRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<CourseMaterial?> AddCourseMaterialAsync(int courseId, CourseMaterial courseMaterial)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return null; // Course not found
            }
            courseMaterial.CourseId = courseId; // Associate the material with the course
            courseMaterial.CreatedDate = DateTime.UtcNow; // Set the creation date
            _context.CourseMaterials.Add(courseMaterial); // Adds the material to the context
            await _context.SaveChangesAsync();
            return courseMaterial; // Returns the added material
        }

        public async Task<bool> DeleteCourseMaterialAsync(int courseId, int materialId)
        {
            var courseMaterial = await _context.CourseMaterials
                .Where(cm => cm.Id == materialId && cm.CourseId == courseId)
                .FirstOrDefaultAsync(); // Finds the material by ID and course ID
            if (courseMaterial == null)
            {
                return false; // Material not found
            }
            _context.CourseMaterials.Remove(courseMaterial); // Removes the material from the context

            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected
        }

        public async Task<IEnumerable<CourseMaterial>> GetAllCourseMaterials()
        {
            return await _context.CourseMaterials.ToListAsync(); // Retrieves all course materials from the database
        }

        public async Task<CourseMaterial> GetCourseMaterialByIdAsync(int materialId)
        {
            return await _context.CourseMaterials
                .Where(cm => cm.Id == materialId)
                .FirstOrDefaultAsync(); // Retrieves a material by its ID
        }

        public async Task<IEnumerable<CourseMaterial>> GetCourseMaterialsByCourseAsync(int courseId)
        {
            return await _context.CourseMaterials
                .Where(cm => cm.CourseId == courseId)
                .ToListAsync(); // Retrieves all materials for a specific course
        }

        public async Task<IEnumerable<CourseMaterial>> GetCourseMaterialsByTypeAsync(string type)
        {
            return await _context.CourseMaterials
                .Where(cm => cm.Type != null && cm.Type.ToLower() == type.ToLower())
                .ToListAsync(); // Retrieves materials by type
        }

        public async Task<bool> UpdateCourseMaterialAsync(int courseId, int materialId, CourseMaterial courseMaterial)
        {
            var existingMaterial = await _context.CourseMaterials
                .Where(cm => cm.Id == materialId && cm.CourseId == courseId)
                .FirstOrDefaultAsync(); // Finds the material by ID and course ID

            if (existingMaterial == null)
            {
                return false; // Material not found
            }
            // Update the existing material's properties
            existingMaterial.Type = courseMaterial.Type;
            existingMaterial.Title = courseMaterial.Title;
            existingMaterial.Url = courseMaterial.Url;
            existingMaterial.Description = courseMaterial.Description;
            existingMaterial.SortOrder = courseMaterial.SortOrder;

            return await _context.SaveChangesAsync() > 0; // Returns true if at least one row was affected

        }
    }
}
