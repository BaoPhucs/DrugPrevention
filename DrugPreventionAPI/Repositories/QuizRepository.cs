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
        private readonly ICertificateRepository _certRepo;
        public QuizRepository(DataContext context, ICertificateRepository certificateRepository)
        {
            _context = context;
            _certRepo = certificateRepository;
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
                              && q.Category == course.Category
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
            // --- 0) Lấy enrollment để kiểm tra số lần làm quiz ---
            var enroll = await _context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.MemberId == memberId);
            if (enroll == null)
                throw new InvalidOperationException("Bạn chưa đăng ký khóa học này");
            if (enroll.QuizAttemptCount >= 3)
                throw new InvalidOperationException("Bạn đã hết số lần làm quiz cho khóa học này");

            // --- 1) Tạo QuizSubmission ---
            var submission = new QuizSubmission
            {
                CourseId = courseId,
                MemberId = memberId,
                SubmissionDate = DateTime.UtcNow
            };
            _context.QuizSubmissions.Add(submission);
            await _context.SaveChangesAsync(); // để có submission.Id

            // --- 2) Lưu từng answer và tính score ---
            int score = 0;
            foreach (var a in answers)
            {
                _context.QuizSubmissionAnswers.Add(new QuizSubmissionAnswer
                {
                    SubmissionId = submission.Id,
                    QuestionId = a.QuestionId,
                    OptionId = a.OptionId
                });

                var opt = await _context.QuestionOptions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == a.OptionId && o.QuestionId == a.QuestionId);
                if (opt != null && opt.ScoreValue > 0)
                    score++;
            }

            // --- 3) Cập nhật score & passedStatus ---
            submission.Score = score;
            submission.PassedStatus = score >= (await _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => c.PassingScore)
                .FirstAsync());

            // --- 4) Tăng số lần làm quiz ---
            enroll.QuizAttemptCount++;
            _context.CourseEnrollments.Update(enroll);

            // --- 5) Lưu tất cả thay đổi (score, passedStatus, attemptCount, answers) ---
            await _context.SaveChangesAsync();

            // --- 6) Nếu pass, cập nhật enrollment.Status và tạo certificate ---
            if (submission.PassedStatus == true)
            {
                enroll.Status = "Completed";
                _context.CourseEnrollments.Update(enroll);

                // Tạo certificate
                var certNo = $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 20).ToUpper();
                var certificate = new Certificate
                {
                    MemberId = memberId,
                    CourseId = courseId,
                    CertificateNo = certNo,
                    IssuedDate = DateTime.UtcNow
                };
                await _certRepo.CreateAsync(certificate);

                await _context.SaveChangesAsync();
            }

            return submission;
        }
    }
}
