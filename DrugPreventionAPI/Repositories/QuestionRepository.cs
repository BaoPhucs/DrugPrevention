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

        //public async Task<bool> CreateAsync(QuestionBank q)
        //{
        //    _context.QuestionBanks.Add(q);
        //    return await _context.SaveChangesAsync() > 0;
        //}


        public async Task<QuestionBank> CreateAsync(QuestionBank question)
        {
            // 1) Thêm câu hỏi vào QuestionBank
            await _context.QuestionBanks.AddAsync(question);
            await _context.SaveChangesAsync(); // để question.Id được gán

            // 2) Lấy tất cả khóa học cùng level
            var courses = await _context.Courses
                .Where(c => c.Level == question.Level)
                .Select(c => c.Id)
                .ToListAsync();

            // 3) Tạo bản ghi trung gian CourseQuestion cho mỗi course
            foreach (var courseId in courses)
            {
                _context.CourseQuestions.Add(new CourseQuestion
                {
                    CourseId = courseId,
                    QuestionId = question.Id
                });
            }
            await _context.SaveChangesAsync();

            return question;
        }

        public async Task<IEnumerable<QuestionBank>> CreateRangeAsync(IEnumerable<QuestionBank> questions)
        {
            // Tạo tất cả QuestionBank trước
            await _context.QuestionBanks.AddRangeAsync(questions);
            await _context.SaveChangesAsync();

            // Lấy danh sách id và levels vừa tạo
            var created = questions.ToList();
            // Lấy toàn bộ các khóa học
            var allCourses = await _context.Courses.ToListAsync();

            foreach (var q in created)
            {
                var matched = allCourses
                    .Where(c => c.Level == q.Level)
                    .Select(c => c.Id);
                foreach (var courseId in matched)
                {
                    _context.CourseQuestions.Add(new CourseQuestion
                    {
                        CourseId = courseId,
                        QuestionId = q.Id
                    });
                }
            }
            await _context.SaveChangesAsync();
            return created;
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
