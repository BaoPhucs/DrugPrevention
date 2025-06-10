using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class ConsultationNote
{
    public int Id { get; set; }

    public int? AppointmentId { get; set; }

    public int? ConsultantId { get; set; }

    public int? MemberId { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedDate { get; set; }

    public AppointmentRequest? Appointment { get; set; }

    public User? Consultant { get; set; }

    public User? Member { get; set; }
}
