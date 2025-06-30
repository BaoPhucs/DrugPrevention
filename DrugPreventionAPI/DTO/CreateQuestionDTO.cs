namespace DrugPreventionAPI.DTO
{
    public class CreateQuestionDTO
    {
        public string QuestionText { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string Category { get; set; } = null!;

    }
}
