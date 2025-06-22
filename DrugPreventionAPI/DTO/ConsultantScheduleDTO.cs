namespace DrugPreventionAPI.DTO
{
    public class ConsultantScheduleDTO
    {
        public int Id { get; set; }  // để Update/Delete
        public DateOnly ScheduleDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
