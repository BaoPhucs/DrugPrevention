namespace DrugPreventionAPI.DTO
{
    public class BlogPostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? CreatedById { get; set; }
        public string? Status { get; set; }
        public string? ReviewComments { get; set; }
        public IEnumerable<TagDTO> Tags { get; set; } = Array.Empty<TagDTO>();
        public IEnumerable<CommentDTO>? Comments { get; set; }


    }
}
