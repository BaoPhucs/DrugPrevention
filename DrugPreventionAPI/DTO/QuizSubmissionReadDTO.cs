namespace DrugPreventionAPI.DTO
{
    public class QuizSubmissionReadDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int MemberId { get; set; }
        public int Score { get; set; }
        public bool PassedStatus { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
}
