using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class ActivityParticipation
{
    public int Id { get; set; }

    public int? ProgramId { get; set; }

    public int? MemberId { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public int? SurveyId { get; set; }

    public User? Member { get; set; }

    public CommunicationActivity? Program { get; set; }
}
