namespace DrugPreventionAPI.Models
{
    public class CourseQuestion
    {
        // Các khóa ngoại
        public int CourseId { get; set; }
        public int QuestionId { get; set; }

        // Navigation
        public Course Course { get; set; } = null!;
        public QuestionBank Question { get; set; } = null!;
    }
}
