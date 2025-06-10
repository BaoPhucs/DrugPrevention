using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class Survey
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Questions { get; set; }

    public string? ScoringRules { get; set; }

    public int? CreatedById { get; set; }

    public User? CreatedBy { get; set; }

    public ICollection<SurveySubmission> SurveySubmissions { get; set; }

    public ICollection<UserSurvey> UserSurveys { get; set; } 
}
