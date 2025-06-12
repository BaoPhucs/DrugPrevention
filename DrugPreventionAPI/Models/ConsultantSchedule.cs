using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class ConsultantSchedule
{
    public int Id { get; set; }

    public int? ConsultantId { get; set; }

    public DateOnly? ScheduleDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ICollection<AppointmentRequest> AppointmentRequests { get; set; } 

    public User? Consultant { get; set; }
}
