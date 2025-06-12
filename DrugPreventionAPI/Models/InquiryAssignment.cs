using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class InquiryAssignment
{
    public int Id { get; set; }

    public int? InquiryId { get; set; }

    public int? AssignedToId { get; set; }

    public int? AssignedById { get; set; }

    public DateTime? AssignedDate { get; set; }

    public string? Priority { get; set; }

    public bool? IsActive { get; set; }

    public User? AssignedBy { get; set; }

    public User? AssignedTo { get; set; }

    public UserInquiry? Inquiry { get; set; }
}
