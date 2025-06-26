namespace DrugPreventionAPI.DTO
{
    public class AppointmentRequestDTO
    {
        public int Id { get; set; }

        public int? MemberId { get; set; }

        public int? ConsultantId { get; set; }

        public int? ScheduleId { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
