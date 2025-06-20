namespace DrugPreventionAPI.Models
{
    public class BlogTag
    {
        public int BlogPostId { get; set; }
        public int TagId { get; set; }

        // Navigation properties
        public BlogPost BlogPost { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
