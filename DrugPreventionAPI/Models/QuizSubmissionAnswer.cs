using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class QuizSubmissionAnswer
{
    public int Id { get; set; }

    public int? SubmissionId { get; set; }

    public int? QuestionId { get; set; }

    public int? OptionId { get; set; }

    public QuestionOption? Option { get; set; }

    public QuestionBank? Question { get; set; }

    public QuizSubmission? Submission { get; set; }
}
