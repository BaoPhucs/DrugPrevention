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
        public async Task<Comment> AddBlogPostCommentAsync(CreateBlogPostCommentDTO dto, int memberId)
        {
            if (dto.Content == null)
                throw new ArgumentException("Content is required.");

            var comment = new Comment
            {
                BlogPostId = dto.BlogPostId,
                Content = dto.Content,
                MemberId = memberId,
                CreatedDate = DateTime.UtcNow
            };

            _ctx.Comments.Add(comment);
            await _ctx.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> AddActivityCommentAsync(CreateActivityCommentDTO dto, int memberId)
        {
            if (dto.Content == null)
                throw new ArgumentException("Content is required.");

            var comment = new Comment
            {
                ActivityId = dto.ActivityId,
                Content = dto.Content,
                MemberId = memberId,
                CreatedDate = DateTime.UtcNow
            };

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
            using var transaction = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var comment = await _ctx.Comments
                    .Include(c => c.Replies) // Đảm bảo tải tất cả các replies
                    .FirstOrDefaultAsync(c => c.Id == id)
                    ?? throw new KeyNotFoundException($"Comment with ID {id} not found.");

                await DeleteRepliesRecursiveAsync(comment);

                _ctx.Comments.Remove(comment);
                await _ctx.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task DeleteRepliesRecursiveAsync(Comment comment)
        {
            if (comment.Replies == null || !comment.Replies.Any()) return;

            // Lấy tất cả các comment con trực tiếp và tải đệ quy
            var replies = await _ctx.Comments
                .Where(c => c.ParentCommentId == comment.Id)
                .Include(c => c.Replies) // Đệ quy tải các comment con
                .ToListAsync();

            foreach (var reply in replies)
            {
                await DeleteRepliesRecursiveAsync(reply); // Xóa đệ quy các comment con
                _ctx.Comments.Remove(reply); // Xóa comment con sau khi xử lý
            }

            await _ctx.SaveChangesAsync(); // Lưu thay đổi sau mỗi cấp
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _ctx.Comments
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment> AddReplyAsync(Comment reply)
        {
            var parent = await _ctx.Comments.FindAsync(reply.ParentCommentId);
            if (parent == null)
                throw new ArgumentException("Parent comment not found.");

            _ctx.Comments.Add(reply);
            await _ctx.SaveChangesAsync();
            return reply;
        }


    }
}
