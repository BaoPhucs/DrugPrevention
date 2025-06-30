namespace DrugPreventionAPI.DTO
{
    public class ConsultationNoteDTO
    {
        public int Id { get; set; }

        public int? AppointmentId { get; set; }

        public int? ConsultantId { get; set; }

        public int? MemberId { get; set; }

        public string? Notes { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
