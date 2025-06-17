namespace DrugPreventionAPI.DTO
{
    public class CreateInquiryAssignment
    {
        public int? InquiryId { get; set; }
        public int? AssignedById { get; set; }
        public int? AssignedToId { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string? Priority { get; set; }
        public bool? IsActive { get; set; }
    }
}
