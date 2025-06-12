using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class SurveySubmissionAnswer
{
    public int Id { get; set; }

    public int? SubmissionId { get; set; }

    public int? QuestionId { get; set; }

    public int? OptionId { get; set; }

    public SurveyOption? Option { get; set; }

    public SurveyQuestion? Question { get; set; }

    public SurveySubmission? Submission { get; set; }
}
