using DrugPreventionAPI.Data;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly DataContext _context;
        public QuizRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuizQuestionDTO>> GetQuizQuestionsAsync(int courseId, int count)
        {
            // 1. Lấy level của course
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) throw new KeyNotFoundException("Course not found");

            // 2. Join CourseQuestion + QuestionBank, lọc theo level
            var query = from cq in _context.CourseQuestions
                        join q in _context.QuestionBanks on cq.QuestionId equals q.Id
                        where cq.CourseId == courseId
                              && q.Level == course.Level
                        select new
                        {
                            q.Id,
                            q.QuestionText
                        };

            // 3. Randomize + take count
            var questions = await query
                .OrderBy(_ => Guid.NewGuid())
                .Take(count)
                .ToListAsync();

            // 4. Với mỗi question, load options và map
            var result = new List<QuizQuestionDTO>();
            foreach (var q in questions)
            {
                var opts = await _context.QuestionOptions
                    .Where(o => o.QuestionId == q.Id)
                    .Select(o => new QuizOptionDTO
                    {
                        Id = o.Id,
                        OptionText = o.OptionText!
                    })
                    .ToListAsync();

                result.Add(new QuizQuestionDTO
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText!,
                    Options = opts
                });
            }

            return result;
        }

        public async Task<QuizSubmission> GetSubmissionByIdAsync(int submissionId)
        {
            return await _context.QuizSubmissions
                .Include(s => s.QuizSubmissionAnswers).ThenInclude(a => a.Option)
                .FirstOrDefaultAsync(s => s.Id == submissionId)
                ?? throw new KeyNotFoundException("Submission not found");
        }

        public async Task<IEnumerable<QuizSubmission>> GetSubmissionsByCourseAsync(int courseId)
        {
            return await _context.QuizSubmissions
                .Where(s => s.CourseId == courseId)
                .OrderByDescending(s => s.SubmissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizSubmission>> GetSubmissionsByUserAsync(int memberId)
        {
            return await _context.QuizSubmissions
                .Where(s => s.MemberId == memberId)
                .OrderByDescending(s => s.SubmissionDate)
                .ToListAsync();
        }

        public async Task<QuizSubmission> SubmitQuizAsync(int courseId, int memberId, IEnumerable<QuizAnswerSubmissionDTO> answers)
        {
            // 1) Tạo QuizSubmission
            var submission = new QuizSubmission
            {
                CourseId = courseId,
                MemberId = memberId,
                SubmissionDate = DateTime.UtcNow
            };
            _context.QuizSubmissions.Add(submission);
            await _context.SaveChangesAsync(); // để có submission.Id

            // 2) Lưu từng answer và tính score
            int score = 0;
            foreach (var a in answers)
            {
                // Lưu detail
                _context.QuizSubmissionAnswers.Add(new QuizSubmissionAnswer
                {
                    SubmissionId = submission.Id,
                    QuestionId = a.QuestionId,
                    OptionId = a.OptionId
                });

                // Kiểm tra đúng/sai: nếu option.ScoreValue > 0 thì +1
                var opt = await _context.QuestionOptions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == a.OptionId && o.QuestionId == a.QuestionId);
                if (opt != null && opt.ScoreValue > 0)
                    score++;
            }

            // 3) Cập nhật score & passedStatus
            submission.Score = score;
            submission.PassedStatus = score >= (await _context.Courses
                                                    .Where(c => c.Id == courseId)
                                                    .Select(c => c.PassingScore)
                                                    .FirstAsync() ?? 0);

            await _context.SaveChangesAsync();

            // 4) Nếu pass, cập nhật CourseEnrollment.Status
            if (submission.PassedStatus == true)
            {
                var enroll = await _context.CourseEnrollments
                    .FirstOrDefaultAsync(e => e.CourseId == courseId && e.MemberId == memberId);
                if (enroll != null)
                {
                    enroll.Status = "Completed";
                    await _context.SaveChangesAsync();
                }
            }

            return submission;
        }
    }
}
