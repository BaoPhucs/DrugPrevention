namespace DrugPreventionAPI.DTO
{
    public class SurveyOptionDTO
    {
        public int Id { get; set; }
        public int? Sequence { get; set; }
        public string? OptionText { get; set; }
        public int? ScoreValue { get; set; }
    }
}
