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
            var questions = await (from q in _context.SurveyQuestions
                                   join s in _context.SurveySubstances on q.SubstanceId equals s.Id
                                   where q.SurveyId == surveyId
                                   orderby s.SortOrder, q.Sequence
                                   select new SurveyQuestionDTO
                                   {
                                       Id = q.Id,
                                       SubstanceId = s.Id,
                                       SubstanceName = s.Name,
                                       Sequence = q.Sequence,
                                       QuestionText = q.QuestionText!,
                                       Options = q.SurveyOptions
                                                  .Select(o => new SurveyOptionDTO
                                                  {
                                                      Id = o.Id,
                                                      OptionText = o.OptionText!,
                                                      ScoreValue = o.ScoreValue
                                                  })
                                                  .ToList()
                                   })
                              .ToListAsync();
            return questions;
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

            // 2) Xác định Recommendation dựa trên survey.Type + risk
            string rec;
            if (survey.Type == "ASSIST")
            {
                rec = risk switch
                {
                    "Low" => "Lời khuyên: Bạn đang ở mức thấp, có thể tiếp tục sử dụng với mức độ hiện tại nhưng nên tự theo dõi. Nguy cơ phát triển lệ thuộc thấp.",
                    "Medium" => "Gợi ý: Bạn ở mức trung bình, tham khảo thêm thông tin về tác hại và cân nhắc giảm sử dụng. Nguy cơ tổn thương gan, thần kinh và lệ thuộc vừa phải.",
                    "High" => "Khuyến nghị: Bạn ở mức cao, nên sắp xếp buổi tư vấn chuyên sâu với chuyên gia. Nguy cơ lệ thuộc nặng, tổn thương sức khỏe dài hạn rất cao.",
                    _ => ""
                };
            }
            else // CRAFTT
            {
                rec = risk switch
                {
                    "Low" => "Kết quả thấp: Tình hình tốt, tiếp tục duy trì lối sống lành mạnh. Nguy cơ tai nạn hoặc tổn thương sức khỏe rất thấp.",
                    "Medium" => "Kết quả trung bình: Tìm hiểu nguyên nhân và cách hạn chế. Nguy cơ rối loạn hành vi, tai nạn do ảnh hưởng chất có hại vừa phải.",
                    "High" => "Kết quả cao: Nên dừng sử dụng để bảo vệ sức khỏe, cân nhắc gặp chuyên gia. Nguy cơ ngộ độc, rối loạn tâm thần và tổn thương cơ quan rất cao.",
                    _ => ""
                };
            }

            // lưu
            var sub = new SurveySubmission
            {
                SurveyId = surveyId,
                MemberId = memberId,
                Score = totalScore,
                RiskLevel = risk,
                Recommendation = rec,
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
