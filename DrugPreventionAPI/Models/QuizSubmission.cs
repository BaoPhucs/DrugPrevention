using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class QuizSubmission
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? MemberId { get; set; }

    public int? Score { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public bool? PassedStatus { get; set; }

    public Course? Course { get; set; }

    public User? Member { get; set; }

    public ICollection<QuizSubmissionAnswer> QuizSubmissionAnswers { get; set; }
}
