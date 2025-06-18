namespace DrugPreventionAPI.DTO
{
    public class CreateSurveyOptionDTO
    {
        public int QuestionId { get; set; }
        public int Sequence { get; set; }
        public string OptionText { get; set; } = null!;
        public int ScoreValue { get; set; }
    }
}
