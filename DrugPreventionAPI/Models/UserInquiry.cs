using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class UserInquiry
{
    public int Id { get; set; }

    public string? Subject { get; set; }

    public int? CreatedById { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastUpdated { get; set; }

    public User? CreatedBy { get; set; }

    public ICollection<InquiryAssignment> InquiryAssignments { get; set; } 

    public ICollection<InquiryComment> InquiryComments { get; set; } 
}
