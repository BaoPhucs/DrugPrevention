namespace DrugPreventionAPI.DTO
{
    public class CreateBlogPostDTO
    {
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string? CoverImageUrl { get; set; }
        public List<int> TagIds { get; set; } = new();

    }
}
