using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class AppointmentRequest
{
    public int Id { get; set; }

    public int? MemberId { get; set; }

    public int? ConsultantId { get; set; }

    public int? ScheduleId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CancelledDate { get; set; }

    public string? CancelReason { get; set; }

    public int? NoShowCount { get; set; }

    public DateTime? NoShowDate { get; set; }

    public User? Consultant { get; set; }

    public virtual ICollection<ConsultationNote> ConsultationNotes { get; set; }

    public User? Member { get; set; }

    public ConsultantSchedule? Schedule { get; set; }
}
