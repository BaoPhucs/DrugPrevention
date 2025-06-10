using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class UserSurvey
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? SurveyId { get; set; }

    public string? RoleInSurvey { get; set; }

    public int? SubmissionId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public SurveySubmission? Submission { get; set; }

    public Survey? Survey { get; set; }

    public User? User { get; set; }
}
