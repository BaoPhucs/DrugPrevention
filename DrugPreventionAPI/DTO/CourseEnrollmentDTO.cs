namespace DrugPreventionAPI.DTO
{
    public class CourseEnrollmentDTO
    {
        public int CourseId { get; set; }
        public int MemberId { get; set; }
        public DateTime ParticipationDate { get; set; }
        public string? Status { get; set; }
    }
}
