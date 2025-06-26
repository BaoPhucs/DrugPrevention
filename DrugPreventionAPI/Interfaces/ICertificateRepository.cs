using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ICertificateRepository
    {
        Task<Certificate> CreateAsync(Certificate cert);
        Task<Certificate?> GetByIdAsync(int id);
        Task<IEnumerable<Certificate>> GetByMemberAsync(int memberId);
        Task<IEnumerable<Certificate>> GetByCourseAsync(int courseId);
    }
}
