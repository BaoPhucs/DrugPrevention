﻿using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class Course
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }

    public string? Category { get; set; }

    public string? Level { get; set; }

    public int? Duration { get; set; }

    public int? PassingScore { get; set; }

    public string? Status { get; set; } = "Pending";

    public int? CreatedById { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ReviewComments { get; set; }

    public int? UpdateById { get; set; }

    public DateTime? UpdateDate { get; set; }

    public ICollection<CourseEnrollment> CourseEnrollments { get; set; }

    public ICollection<CourseMaterial> CourseMaterials { get; set; }

    public ICollection<CourseQuestion> CourseQuestions { get; set; }

    public User? CreatedBy { get; set; }

    public ICollection<QuizSubmission> QuizSubmissions { get; set; } 

    public ICollection<QuestionBank> Questions { get; set; } 
}
