using DrugPreventionAPI.Data;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class TagRepo : ITagRepo
    {
        private readonly DataContext _ctx;
        public TagRepo(DataContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Tag>> GetAllAsync()
            => await _ctx.Tags.ToListAsync();

        public async Task<Tag?> GetByIdAsync(int id)
            => await _ctx.Tags.FindAsync(id);

        public async Task<Tag> AddAsync(Tag tag)
        {
            _ctx.Tags.Add(tag);
            await _ctx.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag> UpdateAsync(Tag tag)
        {
            _ctx.Tags.Update(tag);
            await _ctx.SaveChangesAsync();
            return tag;
        }

        public async Task DeleteAsync(int id)
        {
            var t = await _ctx.Tags.FindAsync(id);
            if (t != null) { _ctx.Tags.Remove(t); await _ctx.SaveChangesAsync(); }
        }
    }
}
