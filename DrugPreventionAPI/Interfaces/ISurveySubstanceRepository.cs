using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ISurveySubstanceRepository
    {
        Task<IEnumerable<SurveySubstance>> GetBySurveyAsync(int surveyId);
        Task<SurveySubstance?> GetByIdAsync(int id);
        Task<SurveySubstance> CreateAsync(SurveySubstance substance);
        Task<bool> UpdateAsync(SurveySubstance substance);
        Task<bool> DeleteAsync(int id);
    }
}
