using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class QuestionBank
{
    public int Id { get; set; }

    public string? QuestionText { get; set; }

    public string? Level { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ICollection<CourseQuestion> CourseQuestions { get; set; }

    public ICollection<QuestionOption> QuestionOptions { get; set; } 

    public ICollection<QuizSubmissionAnswer> QuizSubmissionAnswers { get; set; } 

    public ICollection<Course> Courses { get; set; } 
}
