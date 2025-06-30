namespace DrugPreventionAPI.DTO
{
    public class SurveySubmissionReadDTO
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int MemberId { get; set; }
        public int Score { get; set; }
        public string RiskLevel { get; set; } // Low/Medium/High
        public string Recommendation { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
}
