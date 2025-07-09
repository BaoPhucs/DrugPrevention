namespace DrugPreventionAPI.DTO
{
    public class CreateBlogPostCommentDTO
    {
        public int? BlogPostId { get; set; }
        public string Content { get; set; } = null!;
    }
}
