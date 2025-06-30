namespace DrugPreventionAPI.DTO
{
    public class SurveyQuestionDTO
    {
        public int Id { get; set; }
        public int? Sequence { get; set; }
        public string? QuestionText { get; set; }
        public int? SubstanceId { get; set; }
        public string SubstanceName { get; set; }
        public List<SurveyOptionDTO>? Options { get; set; }
    }
}
