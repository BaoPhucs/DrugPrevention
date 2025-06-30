namespace DrugPreventionAPI.Models
{
    public class SurveySubstance
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string Name { get; set; } = null!;
        public int SortOrder { get; set; }

        public Survey Survey { get; set; } = null!;
        public ICollection<SurveyQuestion> Questions { get; set; }
    }
}
