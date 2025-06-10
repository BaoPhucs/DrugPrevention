using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class UserInquiryMessage
{
    public int Id { get; set; }

    public int? InquiryId { get; set; }

    public int? SenderId { get; set; }

    public string? Message { get; set; }

    public DateTime? Timestamp { get; set; }

    public UserInquiry? Inquiry { get; set; }

    public User? Sender { get; set; }
}
