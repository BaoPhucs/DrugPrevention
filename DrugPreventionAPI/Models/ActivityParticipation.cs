using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class ActivityParticipation
{
    public int Id { get; set; }

    public int? ActivityId { get; set; }

    public int? MemberId { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public string? Status { get; set; }

    public CommunicationActivity? Activity { get; set; }

    public User? Member { get; set; }
}
