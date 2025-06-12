using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class CourseMaterial
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public string? Type { get; set; }

    public string? Title { get; set; }

    public string? Url { get; set; }

    public string? Description { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Course? Course { get; set; }
}
