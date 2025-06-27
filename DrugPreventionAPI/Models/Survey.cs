using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class Survey
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public int? CreatedById { get; set; }

    public DateTime? CreatedDate { get; set; }

    public User? CreatedBy { get; set; }

    public ICollection<SurveyQuestion> SurveyQuestions { get; set; } 

    public ICollection<SurveySubmission> SurveySubmissions { get; set; } 

    public ICollection<UserSurvey> UserSurveys { get; set; }

    public ICollection<SurveySubstance> SurveySubstances { get; set; }
}
