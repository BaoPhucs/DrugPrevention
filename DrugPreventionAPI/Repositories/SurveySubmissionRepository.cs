using DrugPreventionAPI.Data;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class SurveySubmissionRepository : ISurveySubmissionRepository
    {
        private readonly DataContext _context;
        public SurveySubmissionRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<SurveySubmission?> GetByIdAsync(int submissionId)
        {
            return await _context.SurveySubmissions
                         .Include(s => s.SurveySubmissionAnswers)
                         .FirstOrDefaultAsync(s => s.Id == submissionId);
        }

        public async Task<IEnumerable<SurveySubmission>> GetBySurveyAsync(int surveyId)
        {
            return await _context.SurveySubmissions
                         .Where(s => s.SurveyId == surveyId)
                         .ToListAsync();
        }

        public async Task<IEnumerable<SurveySubmission>> GetByUserAsync(int memberId)
        {
            return await _context.SurveySubmissions
                             .Where(s => s.MemberId == memberId)
                             .OrderByDescending(s => s.SubmissionDate)
                             .ToListAsync();
        }

        public async Task<IEnumerable<SurveyQuestionDTO>> GetQuestionsForSubmissionAsync(int surveyId)
        {
            // load questions + options
            var qs = await _context.SurveyQuestions
                           .Where(q => q.SurveyId == surveyId)
                           .Include(q => q.SurveyOptions.OrderBy(o => o.Sequence))
                           .OrderBy(q => q.Sequence)
                           .ToListAsync();

            // map manually or via AutoMapper in service/controller
            // here return domain, mapping in controller
            return qs.Select(q => new SurveyQuestionDTO
            {
                Id = q.Id,
                Sequence = q.Sequence,
                QuestionText = q.QuestionText,
                Options = q.SurveyOptions
                            .Select(o => new SurveyOptionDTO
                            {
                                Id = o.Id,
                                Sequence = o.Sequence,
                                OptionText = o.OptionText,
                                ScoreValue = o.ScoreValue
                            })
                            .ToList()
            });
        }

        public async Task<SurveySubmission> SubmitAsync(int surveyId, int memberId, IEnumerable<SurveyAnswerDTO> answers)
        {
            // tính điểm
            var optionIds = answers.Select(a => a.OptionId).ToList();
            var totalScore = await _context.SurveyOptions
                .Where(o => optionIds.Contains(o.Id))
                .SumAsync(o => o.ScoreValue);

            // xác định RiskLevel
            var survey = await _context.Surveys.FindAsync(surveyId);
            string risk;
            if (survey!.Type == "ASSIST")
            {
                risk = totalScore <= 3
                        ? "Low"
                        : totalScore <= 26
                            ? "Medium"
                            : "High";
            }
            else if (survey.Type == "CRAFFT")
            {
                // ví dụ: >=2 => High, ==1 => Medium, 0 => Low
                risk = totalScore == 0
                        ? "Low"
                        : totalScore == 1
                            ? "Medium"
                            : "High";
            }
            else
            {
                risk = "Unknown";
            }

            // lưu
            var sub = new SurveySubmission
            {
                SurveyId = surveyId,
                MemberId = memberId,
                Score = totalScore,
                RiskLevel = risk,
                SubmissionDate = DateTime.UtcNow
            };
            _context.SurveySubmissions.Add(sub);
            await _context.SaveChangesAsync();

            // lưu từng câu trả lời
            foreach (var ans in answers)
            {
                _context.SurveySubmissionAnswers.Add(new SurveySubmissionAnswer
                {
                    SubmissionId = sub.Id,
                    QuestionId = ans.QuestionId,
                    OptionId = ans.OptionId
                });
            }
            await _context.SaveChangesAsync();
            return sub;
        }
    }
}
