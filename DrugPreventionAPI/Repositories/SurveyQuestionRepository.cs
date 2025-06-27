using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class SurveyQuestionRepository : ISurveyQuestionRepository
    {
        private readonly DataContext _context;
        public SurveyQuestionRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateOptionAsync(IEnumerable<SurveyOption> options)
        {
            _context.SurveyOptions.AddRange(options);
            return await _context.SaveChangesAsync() > 0; 
        }

        public async Task<bool> CreateQuestionAsync(SurveyQuestion question)
        {
            _context.SurveyQuestions.Add(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOptionAsync(int optionId)
        {
            var option = await _context.SurveyOptions.FindAsync(optionId);
            if (option == null)
            {
                return false; // Option not found
            }
            _context.SurveyOptions.Remove(option);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var ex = await _context.SurveyQuestions.FindAsync(questionId);
            if (ex == null) return false;
            _context.SurveyQuestions.Remove(ex);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SurveyQuestion>> GetBySurveyAsync(int surveyId)
        {
            return await _context.SurveyQuestions
                  .Include(q => q.Substance)
                  .Include(q => q.SurveyOptions)
                  .Where(q => q.SurveyId == surveyId)
                  .OrderBy(q => q.Substance!.SortOrder)
                  .ThenBy(q => q.Sequence)
                  .ToListAsync();

        }

        public async Task<SurveyOption?> GetOptionByIdAsync(int optionId)
        {
            return await _context.SurveyOptions.FindAsync(optionId);
        }

        public async Task<IEnumerable<SurveyOption>> GetOptionsAsync(int questionId)
        {
            return await _context.SurveyOptions
                         .Where(o => o.QuestionId == questionId)
                         .OrderBy(o => o.Sequence)
                         .ToListAsync();
        }

        public async Task<SurveyQuestion?> GetQuestionByIdAsync(int questionId)
        {
            return await _context.SurveyQuestions
                  .Include(q => q.Substance)
                  .Include(q => q.SurveyOptions)
                  .FirstOrDefaultAsync(q => q.Id == questionId);
        }

        public async Task<bool> UpdateOptionAsync(SurveyOption option)
        {
            _context.SurveyOptions.Update(option);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateQuestionAsync(SurveyQuestion question)
        {
            var ex = await _context.SurveyQuestions.FindAsync(question.Id);
            if (ex == null) return false;
            ex.QuestionText = question.QuestionText;
            ex.Sequence = question.Sequence;
            ex.SubstanceId = question.SubstanceId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
