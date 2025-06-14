using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ICourseMaterialRepository
    {
        // Catalog
        Task<IEnumerable<CourseMaterial>> GetAllCourseMaterials();
        Task<IEnumerable<CourseMaterial>> GetCourseMaterialsByCourseAsync(int courseId); // Retrieves all materials for a course
        Task<CourseMaterial> GetCourseMaterialByIdAsync(int materialId); // Retrieves a material by its ID
        Task<IEnumerable<CourseMaterial>> GetCourseMaterialsByTypeAsync(string type); // Retrieves materials by type
        // CRUD
        Task<CourseMaterial?> AddCourseMaterialAsync(int courseId, CourseMaterial courseMaterial); // Adds a new material
        Task<bool> UpdateCourseMaterialAsync(int courseId, int materialId, CourseMaterial courseMaterial); // Updates an existing material
        Task<bool> DeleteCourseMaterialAsync(int courseId, int materialId); // Deletes a material by its ID
    }
}
