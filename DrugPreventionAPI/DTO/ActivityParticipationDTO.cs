namespace DrugPreventionAPI.DTO
{
    public class ActivityParticipationDTO
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int MemberId { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? Status { get; set; }
    }
}
