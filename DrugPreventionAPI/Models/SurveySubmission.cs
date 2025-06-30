using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class SurveySubmission
{
    public int Id { get; set; }

    public int? SurveyId { get; set; }

    public int? MemberId { get; set; }

    public int? Score { get; set; }

    public string? RiskLevel { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public string? Recommendation { get; set; }

    public bool? IsAnonymous { get; set; }

    public User? Member { get; set; }

    public Survey? Survey { get; set; }

    public ICollection<SurveySubmissionAnswer> SurveySubmissionAnswers { get; set; } 

    public ICollection<UserSurvey> UserSurveys { get; set; } 
}
