namespace DrugPreventionAPI.DTO
{
    public class CommunicationActivityDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Location { get; set; }
        public int? Capacity { get; set; }
    }
}
