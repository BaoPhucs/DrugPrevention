using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class CommunicationActivity
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? EventDate { get; set; }

    public string? Location { get; set; }

    public int? Capacity { get; set; }

    public int? CreatedById { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ICollection<ActivityParticipation> ActivityParticipations { get; set; } 

    public User? CreatedBy { get; set; }

    public ICollection<Comment> Comments { get; set; }
}
