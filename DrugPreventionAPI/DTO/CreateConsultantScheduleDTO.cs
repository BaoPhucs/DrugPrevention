namespace DrugPreventionAPI.DTO
{
    public class CreateConsultantScheduleDTO
    {
        public DateOnly ScheduleDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
