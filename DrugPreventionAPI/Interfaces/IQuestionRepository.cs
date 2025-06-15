using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IQuestionRepository
    {
        // QuestionBank
        Task<IEnumerable<QuestionBank>> GetAllAsync();
        Task<QuestionBank?> GetByIdAsync(int id);
        Task<bool> CreateAsync(QuestionBank q);
        Task<bool> UpdateAsync(QuestionBank q);
        Task<bool> DeleteAsync(int id);

        // QuestionOption
        Task<IEnumerable<QuestionOption>> GetOptionsAsync(int questionId);
        Task<QuestionOption?> GetOptionByIdAsync(int questionId, int optionId);
        Task<bool> AddOptionsAsync(int questionId, IEnumerable<QuestionOption> options);
        Task<bool> UpdateOptionAsync(int questionId, QuestionOption opt);
        Task<bool> DeleteOptionAsync(int questionId, int optionId);
    }
}
