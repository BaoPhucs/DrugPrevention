using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public ICollection<BlogPost> BlogPosts { get; set; } 
}
