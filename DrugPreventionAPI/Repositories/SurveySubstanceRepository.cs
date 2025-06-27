using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class SurveySubstanceRepository : ISurveySubstanceRepository
    {
        private readonly DataContext _ctx;
        public SurveySubstanceRepository(DataContext context)
        {
            _ctx = context;
        }
        public async Task<IEnumerable<SurveySubstance>> GetBySurveyAsync(int surveyId) =>
        await _ctx.SurveySubstances
                  .Where(s => s.SurveyId == surveyId)
                  .OrderBy(s => s.SortOrder)
                  .ToListAsync();

        public async Task<SurveySubstance?> GetByIdAsync(int id) =>
            await _ctx.SurveySubstances.FindAsync(id);

        public async Task<SurveySubstance> CreateAsync(SurveySubstance sb)
        {
            _ctx.SurveySubstances.Add(sb);
            await _ctx.SaveChangesAsync();
            return sb;
        }

        public async Task<bool> UpdateAsync(SurveySubstance sb)
        {
            var ex = await _ctx.SurveySubstances.FindAsync(sb.Id);
            if (ex == null) return false;
            ex.Name = sb.Name;
            ex.SortOrder = sb.SortOrder;
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ex = await _ctx.SurveySubstances.FindAsync(id);
            if (ex == null) return false;
            _ctx.SurveySubstances.Remove(ex);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
