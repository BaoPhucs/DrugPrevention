namespace DrugPreventionAPI.DTO
{
    public class CreateCommunicationActivityDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string? Location { get; set; }
        public int? Capacity { get; set; }
    }
}
