using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly DataContext _context;
        public QuestionRepository(DataContext context)
        {
            _context = context;
        }

        // QuestionBank
        public async Task<IEnumerable<QuestionBank>> GetAllAsync()
        {
            return await _context.QuestionBanks
            .Include(q => q.QuestionOptions)
            .ToListAsync();
        }

        public async Task<QuestionBank?> GetByIdAsync(int id)
        {
            return await _context.QuestionBanks
            .Include(q => q.QuestionOptions)      // <— include option
            .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<bool> CreateAsync(QuestionBank q)
        {
            _context.QuestionBanks.Add(q);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(QuestionBank q)
        {
            _context.QuestionBanks.Update(q);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var q = await _context.QuestionBanks.FindAsync(id);
            if (q == null) return false;
            _context.QuestionBanks.Remove(q);
            return await _context.SaveChangesAsync() > 0;
        }

        // QuestionOption
        public Task<IEnumerable<QuestionOption>> GetOptionsAsync(int questionId) =>
            _context.QuestionOptions.Where(o => o.QuestionId == questionId).ToListAsync()
                .ContinueWith(t => (IEnumerable<QuestionOption>)t.Result);

        public Task<QuestionOption?> GetOptionByIdAsync(int questionId, int optionId) =>
            _context.QuestionOptions
                .FirstOrDefaultAsync(o => o.QuestionId == questionId && o.Id == optionId);



        public async Task<bool> AddOptionsAsync(int questionId, IEnumerable<QuestionOption> options)
        {
            if (options == null || !options.Any())
                return false;

            foreach (var opt in options)
            {
                opt.QuestionId = questionId;
                _context.QuestionOptions.Add(opt);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateOptionAsync(int questionId, QuestionOption opt)
        {
            var ex = await GetOptionByIdAsync(questionId, opt.Id);
            if (ex == null) return false;
            ex.OptionText = opt.OptionText;
            ex.ScoreValue = opt.ScoreValue;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOptionAsync(int questionId, int optionId)
        {
            var ex = await GetOptionByIdAsync(questionId, optionId);
            if (ex == null) return false;
            _context.QuestionOptions.Remove(ex);
            return await _context.SaveChangesAsync() > 0;
        }

        
    }
}
