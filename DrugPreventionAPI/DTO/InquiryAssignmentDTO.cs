namespace DrugPreventionAPI.DTO
{
    public class InquiryAssignmentDTO
    {
        public int Id { get; set; }
        public int? InquiryId { get; set; }
        public int? AssignedById { get; set; }
        public int? AssignedToId { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string? Priority { get; set; }
        public bool? IsActive { get; set; }
        public UserDTO? AssignedBy { get; set; }
        public UserDTO? AssignedTo { get; set; }
    }
}
