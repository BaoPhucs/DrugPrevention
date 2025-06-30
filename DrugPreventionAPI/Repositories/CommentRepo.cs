using AutoMapper;
using DrugPreventionAPI.Data;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class CommentRepo : ICommentRepo
    {
        private readonly DataContext _ctx;
        private readonly IMapper _mapper;
        public CommentRepo(DataContext ctx, IMapper mapper) => (_ctx, _mapper) = (ctx, mapper);

        public async Task<IEnumerable<Comment>> GetRepliesAsync(int parentId)
        {
            return await _ctx.Comments
                .Where(c => c.ParentCommentId == parentId)
                .Include(c => c.Replies)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _ctx.Comments
                .Include(c => c.Replies)
                .ToListAsync();
        }
        public async Task<IEnumerable<Comment>> GetAllByPostIdAsync(int blogPostId)
        {
            return await _ctx.Comments
                .Where(c => c.BlogPostId == blogPostId && c.ParentCommentId == null)
                .Include(c => c.Replies)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllByActivityIdAsync(int activityId)
        {
            return await _ctx.Comments
                .Where(c => c.ActivityId == activityId && c.ParentCommentId == null)
                .Include(c => c.Replies)
                .ToListAsync();
        }
        public async Task<Comment> AddAsync(CreateCommentDTO dto)
        {
            // ✅ Validate it's a root comment (not a reply)
            if (dto.BlogPostId == null && dto.ActivityId == null)
                throw new ArgumentException("A root comment must target either a BlogPost or an Activity.");

            if (dto.BlogPostId != null && dto.ActivityId != null)
                throw new ArgumentException("A comment must not target both a BlogPost and an Activity.");

            // Map and save
            var comment = _mapper.Map<Comment>(dto);
            comment.CreatedDate = DateTime.UtcNow;

            _ctx.Comments.Add(comment);
            await _ctx.SaveChangesAsync();

            return comment;
        }




        public async Task<Comment> UpdateAsync(int id, UpdateCommentDTO dto)
        {
            var comment = await _ctx.Comments.FindAsync(id)
                ?? throw new KeyNotFoundException();
            _mapper.Map(dto, comment);
            await _ctx.SaveChangesAsync();
            return comment;
        }
        public async Task DeleteAsync(int id)
        {
            var comment = await _ctx.Comments.FindAsync(id)
                ?? throw new KeyNotFoundException($"Comment with ID {id} not found.");

            _ctx.Comments.Remove(comment);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _ctx.Comments
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment> AddReplyAsync(CreateReplyDTO dto, int memberId)
        {
            var parent = await _ctx.Comments.FindAsync(dto.ParentCommentId);
            if (parent == null)
                throw new ArgumentException("Parent comment not found.");

            var reply = new Comment
            {
                ParentCommentId = dto.ParentCommentId,
                Content = dto.Content,
                CreatedDate = DateTime.UtcNow,
                MemberId = memberId
            };

            _ctx.Comments.Add(reply);
            await _ctx.SaveChangesAsync();
            return reply;
        }


    }
}
