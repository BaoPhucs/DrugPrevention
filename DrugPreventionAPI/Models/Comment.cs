using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int? BlogPostId { get; set; }

    public int? ActivityId { get; set; }

    public int? ParentCommentId { get; set; }

    public int? MemberId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Status { get; set; }

    public ICollection<Comment> Replies { get; set; } 

    public User? Member { get; set; }

    public BlogPost? BlogPost { get; set; }

    public CommunicationActivity? Activity { get; set; }

    public Comment? ParentComment { get; set; }
}
