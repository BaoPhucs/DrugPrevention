namespace DrugPreventionAPI.DTO
{
    public class CreateSurveyQuestionDTO
    {
        public int SurveyId { get; set; }
        public int Sequence { get; set; }
        public string QuestionText { get; set; } = null!;
    }
}
