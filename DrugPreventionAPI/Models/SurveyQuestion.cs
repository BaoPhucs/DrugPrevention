using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class SurveyQuestion
{
    public int Id { get; set; }

    public int? SurveyId { get; set; }

    public int? Sequence { get; set; }

    public string? QuestionText { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Survey? Survey { get; set; }

    public ICollection<SurveyOption> SurveyOptions { get; set; } 

    public ICollection<SurveySubmissionAnswer> SurveySubmissionAnswers { get; set; } 
}
