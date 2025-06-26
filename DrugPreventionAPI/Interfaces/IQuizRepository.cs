using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface IQuizRepository
    {
        // Lấy ngẫu nhiên `count` câu hỏi cho course, filter theo Course.Level
        Task<IEnumerable<QuizQuestionDTO>> GetQuizQuestionsAsync(int courseId, int count);

        // Lưu quiz & tự động cập nhật CourseEnrollment.Status = "Completed" nếu pass
        Task<QuizSubmission> SubmitQuizAsync(int courseId, int memberId, IEnumerable<QuizAnswerSubmissionDTO> answers);

        // Lấy lịch sử submissions của 1 course
        Task<IEnumerable<QuizSubmission>> GetSubmissionsByCourseAsync(int courseId);

        // Lấy lịch sử submissions của 1 user
        Task<IEnumerable<QuizSubmission>> GetSubmissionsByUserAsync(int memberId);

        // Lấy chi tiết 1 submission, bao gồm các QuizSubmissionAnswer
        Task<QuizSubmission> GetSubmissionByIdAsync(int submissionId);
    }
}
