using System.Globalization;

namespace DrugPreventionAPI.DTO
{
    public class UpdateBlogPostDTO
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? CoverImageUrl { get; set; }
        public List<int>? TagIds { get; set; }  // for BlogTag


    }
}
