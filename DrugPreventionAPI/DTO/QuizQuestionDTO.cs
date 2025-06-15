namespace DrugPreventionAPI.DTO
{
    public class QuizQuestionDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = null!;
        public List<QuizOptionDTO> Options { get; set; } = new();
    }
}
