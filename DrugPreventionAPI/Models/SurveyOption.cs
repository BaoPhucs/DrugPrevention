using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class SurveyOption
{
    public int Id { get; set; }

    public int? QuestionId { get; set; }

    public int? Sequence { get; set; }

    public string? OptionText { get; set; }

    public int? ScoreValue { get; set; }

    public DateTime? CreatedDate { get; set; }

    public SurveyQuestion? Question { get; set; }

    public ICollection<SurveySubmissionAnswer> SurveySubmissionAnswers { get; set; } 
}
