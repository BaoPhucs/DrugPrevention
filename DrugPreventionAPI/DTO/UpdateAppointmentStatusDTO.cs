namespace DrugPreventionAPI.DTO
{
    public class UpdateAppointmentStatusDTO
    {
        public string Status { get; set; }    // "Confirmed", "Cancelled"
        public string? CancelReason { get; set; }
    }
}
