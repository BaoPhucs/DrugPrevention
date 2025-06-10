using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public string? Type { get; set; }

    public DateTime? SendDate { get; set; }

    public User? User { get; set; }
}
