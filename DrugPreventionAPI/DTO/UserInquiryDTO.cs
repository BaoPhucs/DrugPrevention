namespace DrugPreventionAPI.DTO
{
    public class UserInquiryDTO
    {
        public int ID { get; set; }
        public string Subject { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public UserDTO CreatedBy { get; set; } = null!;
    }
}
