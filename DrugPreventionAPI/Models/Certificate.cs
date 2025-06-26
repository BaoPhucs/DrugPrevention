namespace DrugPreventionAPI.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int CourseId { get; set; }
        public DateTime IssuedDate { get; set; }
        public string CertificateNo { get; set; } = null!;
        public string? FileUrl { get; set; }

        // navigation
        public User Member { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
