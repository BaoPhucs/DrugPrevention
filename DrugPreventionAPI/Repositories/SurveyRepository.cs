using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly DataContext _context;
        public SurveyRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(Survey survey)
        {
            _context.Surveys.Add(survey);
            return await _context.SaveChangesAsync() > 0; 
        }

        public async Task<bool> DeleteAsync(int surveyId)
        {
            var survey = await _context.Surveys.FindAsync(surveyId);
            if (survey == null)
            {
                return false; 
            }
            _context.Surveys.Remove(survey);
            return await _context.SaveChangesAsync() > 0; 
        }

        public async Task<IEnumerable<Survey>> GetAllAsync()
        {
            return await _context.Surveys.AsNoTracking().ToListAsync();
        }

        public async Task<Survey?> GetByIdAsync(int surveyId)
        {
            return await _context.Surveys.Include(s => s.SurveyQuestions)
                .ThenInclude(q => q.SurveyOptions).AsNoTracking().FirstOrDefaultAsync(s => s.Id == surveyId);
        }

        public async Task<bool> UpdateAsync(Survey survey)
        {
            _context.Surveys.Update(survey);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
