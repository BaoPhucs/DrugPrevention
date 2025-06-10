using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class CommunicationActivity
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public DateTime? EventDate { get; set; }

    public int? CreatedById { get; set; }

    public ICollection<ActivityParticipation> ActivityParticipations { get; set; }

    public User? CreatedBy { get; set; }
}
