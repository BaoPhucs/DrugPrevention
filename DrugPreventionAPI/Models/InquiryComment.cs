using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class InquiryComment
{
    public int Id { get; set; }

    public int? InquiryId { get; set; }

    public int? CommentById { get; set; }

    public string? CommentType { get; set; }

    public string? CommentText { get; set; }

    public string? AttachmentUrl { get; set; }

    public string? FileName { get; set; }

    public string? AttachmentType { get; set; }

    public DateTime? CreatedDate { get; set; }

    public User? CommentBy { get; set; }

    public UserInquiry? Inquiry { get; set; }
}
