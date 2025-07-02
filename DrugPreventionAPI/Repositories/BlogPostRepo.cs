using DrugPreventionAPI.Data;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Repositories
{
    public class BlogPostRepo : IBlogPostRepo

    {
        private readonly DataContext _ctx;
        public BlogPostRepo(DataContext ctx) => _ctx = ctx;

        public async Task<BlogPost> AddAsync(BlogPost post, IEnumerable<int> tagIds)
        {
            // attach post
            post.BlogTags = tagIds
                .Select(tid => new BlogTag { BlogPostId = post.Id, TagId = tid })
                .ToList();

            _ctx.BlogPosts.Add(post);
            await _ctx.SaveChangesAsync();
            return post;
        }

        public async Task DeleteAsync(int id)
        {
            // 1) Remove any tag links
            var links = _ctx.BlogTags.Where(bt => bt.BlogPostId == id);
            _ctx.BlogTags.RemoveRange(links);

            // 2) (Optional) Remove comments too
            var comments = _ctx.Comments.Where(c => c.BlogPostId == id);
            _ctx.Comments.RemoveRange(comments);

            // 3) Remove the post itself
            var bp = await _ctx.BlogPosts.FindAsync(id);
            if (bp != null)
                _ctx.BlogPosts.Remove(bp);

            await _ctx.SaveChangesAsync();
        }


        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        => await _ctx.BlogPosts
                     .Include(bp => bp.BlogTags).ThenInclude(bt => bt.Tag)
                     .Include(bp => bp.Comments).ThenInclude(c => c.Replies)

                     .ToListAsync();

        public async Task<BlogPost?> GetByIdAsync(int id)
      => await _ctx.BlogPosts
                   .Include(bp => bp.BlogTags)
                      .ThenInclude(bt => bt.Tag)
                     .Include(bp => bp.Comments).ThenInclude(c => c.Replies)

                   .FirstOrDefaultAsync(bp => bp.Id == id);

        public async Task<BlogPost> GetByTagId(int tagId)
        {
            return await _ctx.BlogPosts
                .Include(bp => bp.BlogTags)
                    .ThenInclude(bt => bt.Tag)
                .Include(bp => bp.Comments).ThenInclude(c => c.Replies)
                .FirstOrDefaultAsync(bp => bp.BlogTags.Any(bt => bt.TagId == tagId));
        }

        public async Task<BlogPost> UpdateAsync(int postId, UpdateBlogPostDTO dto)
        {
            // Lấy blog post từ database với các navigation properties
            var post = await _ctx.BlogPosts
                .Include(bp => bp.BlogTags)
                .FirstOrDefaultAsync(bp => bp.Id == postId);

            if (post == null) throw new KeyNotFoundException();

            // Chỉ cập nhật các thuộc tính khi giá trị được cung cấp, hợp lệ, và không phải giá trị mặc định
            if (dto.Title != null && !string.IsNullOrWhiteSpace(dto.Title) && dto.Title != "string")
            {
                post.Title = dto.Title;
            }

            if (dto.Content != null && !string.IsNullOrWhiteSpace(dto.Content) && dto.Content != "string")
            {
                post.Content = dto.Content;
            }

            if (dto.CoverImageUrl != null && !string.IsNullOrWhiteSpace(dto.CoverImageUrl) && dto.CoverImageUrl != "string")
            {
                post.CoverImageUrl = dto.CoverImageUrl;
            }

            // Xử lý cập nhật TagIds chỉ khi danh sách không rỗng và không chứa giá trị mặc định không hợp lệ
            if (dto.TagIds != null && dto.TagIds.Any() && !dto.TagIds.All(id => id == 0))
            {
                // Xóa các tag cũ
                var oldTags = _ctx.BlogTags.Where(bt => bt.BlogPostId == postId);
                _ctx.BlogTags.RemoveRange(oldTags);

                // Thêm các tag mới, loại bỏ giá trị 0
                post.BlogTags = dto.TagIds
                    .Where(tagId => tagId != 0)
                    .Select(tagId => new BlogTag { BlogPostId = postId, TagId = tagId })
                    .ToList();
            }

            // Cập nhật ngày cập nhật nếu có thay đổi
            post.UpdatedDate = DateTime.UtcNow;

            await _ctx.SaveChangesAsync();

            // Tải lại dữ liệu đầy đủ để trả về
            return await _ctx.BlogPosts
                .Include(bp => bp.BlogTags)
                .ThenInclude(bt => bt.Tag)
                .Include(bp => bp.Comments)
                .ThenInclude(c => c.Replies)
                .FirstOrDefaultAsync(bp => bp.Id == postId) ?? post;
        }


    }
}
