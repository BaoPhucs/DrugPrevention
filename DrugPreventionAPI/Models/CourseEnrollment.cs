using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class CourseEnrollment
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? MemberId { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public string? Status { get; set; }

    public Course? Course { get; set; }

    public User? Member { get; set; }
}
