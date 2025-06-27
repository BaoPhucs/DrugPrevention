namespace DrugPreventionAPI.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public int? BlogPostId { get; set; }
        public int? ActivityId { get; set; }
        public int? ParentCommentId { get; set; }
        public int? MemberId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Status { get; set; }
        public IEnumerable<CommentDTO>? Replies { get; set; }
    }
}
