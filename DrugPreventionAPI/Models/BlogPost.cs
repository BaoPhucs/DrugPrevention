using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class BlogPost
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Image { get; set; }

    public string? Tags { get; set; }

    public int? CreatedById { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ICollection<Comment> Comments { get; set; }

    public User? CreatedBy { get; set; }
}
