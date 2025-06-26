using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly DataContext _context;
        public CertificateRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Certificate> CreateAsync(Certificate cert)
        {
            _context.Certificates.Add(cert);
            await _context.SaveChangesAsync();
            return cert;
        }

        public async Task<Certificate?> GetByIdAsync(int id)
            => await _context.Certificates.FindAsync(id);

        public async Task<IEnumerable<Certificate>> GetByMemberAsync(int memberId)
            => await _context.Certificates
                             .Where(c => c.MemberId == memberId)
                             .ToListAsync();

        public async Task<IEnumerable<Certificate>> GetByCourseAsync(int courseId)
            => await _context.Certificates
                             .Where(c => c.CourseId == courseId)
                             .ToListAsync();
    }
}
