using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ISurveyRepository
    {
        Task<IEnumerable<Survey>> GetAllAsync();
        Task<Survey?> GetByIdAsync(int surveyId);
        Task<bool> CreateAsync(Survey survey);
        Task<bool> UpdateAsync(Survey survey);
        Task<bool> DeleteAsync(int surveyId);
    }
}
