using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ISurveyQuestionRepository
    {
        // --- SurveyQuestion (Admin) ---
        Task<IEnumerable<SurveyQuestion>> GetBySurveyAsync(int surveyId);
        Task<SurveyQuestion?> GetQuestionByIdAsync(int questionId);
        Task<bool> CreateQuestionAsync(SurveyQuestion question);
        Task<bool> UpdateQuestionAsync(SurveyQuestion question);
        Task<bool> DeleteQuestionAsync(int questionId);

        // --- SurveyOption (Admin) ---
        Task<IEnumerable<SurveyOption>> GetOptionsAsync(int questionId);
        Task<SurveyOption?> GetOptionByIdAsync(int optionId);
        Task<bool> CreateOptionAsync(IEnumerable<SurveyOption> options);
        Task<bool> UpdateOptionAsync(SurveyOption option);
        Task<bool> DeleteOptionAsync(int optionId);
    }
}
