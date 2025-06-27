namespace DrugPreventionAPI.Models
{
    public class CourseEnrollment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int MemberId { get; set; }
        public DateTime ParticipationDate { get; set; }
        public int QuizAttemptCount { get; set; }
        public string? Status { get; set; }

        // navigation properties
        public virtual Course? Course { get; set; }
        public virtual User? Member { get; set; }
    }
}
