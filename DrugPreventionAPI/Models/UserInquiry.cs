using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class UserInquiry
{
    public int Id { get; set; }

    public string? Subject { get; set; }

    public int? CreatedById { get; set; }

    public int? AssignedToId { get; set; }

    public string? Status { get; set; }

    public string? Priority { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastUpdated { get; set; }

    public User? AssignedTo { get; set; }

    public User? CreatedBy { get; set; }

    public ICollection<UserInquiryMessage> UserInquiryMessages { get; set; } = new List<UserInquiryMessage>();
}
