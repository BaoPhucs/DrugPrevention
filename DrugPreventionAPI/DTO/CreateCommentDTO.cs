namespace DrugPreventionAPI.DTO
{
    public class CreateCommentDTO
    {
        public int? BlogPostId { get; set; }
        public int? ActivityId { get; set; }
        public int? ParentCommentId { get; set; }
        public int? MemberId { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }
    }
}
