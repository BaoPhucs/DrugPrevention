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


        public async Task<BlogPost> UpdateAsync(int postId, UpdateBlogPostDTO dto)
        {


            var post = await _ctx.BlogPosts
         .Include(bp => bp.BlogTags)           // need existing tags
         .FirstOrDefaultAsync(bp => bp.Id == postId);

            if (post == null) throw new KeyNotFoundException();

            // map scalars
            post.Title = dto.Title;
            post.Content = dto.Content;
            post.CoverImageUrl = dto.CoverImageUrl;

            // clear old tag links
            var old = _ctx.BlogTags.Where(bt => bt.BlogPostId == postId);
            _ctx.BlogTags.RemoveRange(old);

            // attach new ones
            post.BlogTags = dto.TagIds
                .Select(tid => new BlogTag { BlogPostId = postId, TagId = tid })
                .ToList();

            _ctx.BlogPosts.Update(post);
            await _ctx.SaveChangesAsync();

            return post;
        }


    }
}
