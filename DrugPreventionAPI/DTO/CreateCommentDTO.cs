namespace DrugPreventionAPI.DTO
{
    public class CreateCommentDTO
    {
        public int? BlogPostId { get; set; }
        public int? ActivityId { get; set; }
        public string Content { get; set; } = null!;
    }
}
