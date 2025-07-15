using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ISurveySubmissionRepository
    {
        // Lấy câu hỏi + options (dùng để render form khảo sát cho user)
        Task<IEnumerable<SurveyQuestionDTO>> GetQuestionsForSubmissionAsync(int surveyId);

        // Nộp bài khảo sát & tính toán Score, RiskLevel
        Task<SurveySubmission> SubmitAsync(int surveyId, int memberId, IEnumerable<SurveyAnswerDTO> answers);

        // Lấy lịch sử submission
        Task<IEnumerable<SurveySubmission>> GetBySurveyAsync(int surveyId);
        Task<IEnumerable<SurveySubmission>> GetByUserAsync(int surveyId, int memberId);
        Task<SurveySubmission?> GetByIdAsync(int submissionId);
    }
}
