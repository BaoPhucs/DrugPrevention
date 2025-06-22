namespace DrugPreventionAPI.DTO
{
    public class CourseDTO
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Image { get; set; }

        public string? Description { get; set; }

        public string? Content { get; set; }

        public string? Category { get; set; }

        public string? Level { get; set; }

        public int? Duration { get; set; }

        public int? PassingScore { get; set; }

        public string? Status { get; set; }

        public string? WorkflowState { get; set; }
    }
}
