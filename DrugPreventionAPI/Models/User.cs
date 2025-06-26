using System;
using System.Collections.Generic;

namespace DrugPreventionAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public string? AgeGroup { get; set; }

    public string? ProfileData { get; set; }

    public bool? EmailVerified { get; set; }

    public string? EmailVerificationToken { get; set; }

    public DateTime? EmailVerificationExpiry { get; set; }

    public DateTime? CreatedDate { get; set; }

    public ICollection<Certificate> Certificates { get; set; }

    public ICollection<ActivityParticipation> ActivityParticipations { get; set; } 

    public ICollection<AppointmentRequest> AppointmentRequestConsultants { get; set; }

    public ICollection<AppointmentRequest> AppointmentRequestMembers { get; set; } 

    public ICollection<CourseEnrollment> CourseEnrollments { get; set; }

    public ICollection<BlogPost> BlogPosts { get; set; } 

    public ICollection<Comment> Comments { get; set; } 

    public ICollection<CommunicationActivity> CommunicationActivities { get; set; } 

    public ICollection<ConsultantSchedule> ConsultantSchedules { get; set; }

    public ICollection<ConsultationNote> ConsultationNoteConsultants { get; set; } 

    public ICollection<ConsultationNote> ConsultationNoteMembers { get; set; } 

    public ICollection<Course> Courses { get; set; } 

    public ICollection<InquiryAssignment> InquiryAssignmentAssignedBies { get; set; } 

    public ICollection<InquiryAssignment> InquiryAssignmentAssignedTos { get; set; }

    public ICollection<InquiryComment> InquiryComments { get; set; } 

    public ICollection<Notification> Notifications { get; set; } 

    public ICollection<QuizSubmission> QuizSubmissions { get; set; } 

    public ICollection<SurveySubmission> SurveySubmissions { get; set; }

    public ICollection<Survey> Surveys { get; set; }

    public ICollection<UserInquiry> UserInquiries { get; set; } 

    public ICollection<UserSurvey> UserSurveys { get; set; }
}
