namespace DrugPreventionAPI.DTO
{
    public class QuizSubmissionDetailDTO : QuizSubmissionReadDTO
    {
        public List<QuizAnswerDTO> Answers { get; set; } = new();
    }
}
