namespace DrugPreventionAPI.DTO
{
    public class CourseEnrollmentDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int MemberId { get; set; }
        public DateTime ParticipationDate { get; set; }
        public int QuizAttemptCount { get; set; }
        public string? Status { get; set; }
    }
}
