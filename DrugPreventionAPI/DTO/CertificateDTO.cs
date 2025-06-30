namespace DrugPreventionAPI.DTO
{
    public class CertificateDTO
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int CourseId { get; set; }
        public DateTime IssuedDate { get; set; }
        public string CertificateNo { get; set; } = null!;
        public string? FileUrl { get; set; }
    }
}
