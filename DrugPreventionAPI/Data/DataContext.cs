
using System;
using System.Collections.Generic;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Data;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityParticipation> ActivityParticipations { get; set; }

    public virtual DbSet<AppointmentRequest> AppointmentRequests { get; set; }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommunicationActivity> CommunicationActivities { get; set; }

    public virtual DbSet<ConsultantSchedule> ConsultantSchedules { get; set; }

    public virtual DbSet<ConsultationNote> ConsultationNotes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseEnrollment> CourseEnrollments { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<QuizSubmission> QuizSubmissions { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<SurveySubmission> SurveySubmissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserInquiry> UserInquiries { get; set; }

    public virtual DbSet<UserInquiryMessage> UserInquiryMessages { get; set; }

    public virtual DbSet<UserSurvey> UserSurveys { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityParticipation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Activity__3214EC2706E12022");

            entity.ToTable("ActivityParticipation");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ProgramId).HasColumnName("ProgramID");
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");

            entity.HasOne(d => d.Member).WithMany(p => p.ActivityParticipations)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__ActivityP__Membe__7A672E12");

            entity.HasOne(d => d.Program).WithMany(p => p.ActivityParticipations)
                .HasForeignKey(d => d.ProgramId)
                .HasConstraintName("FK__ActivityP__Progr__797309D9");
        });

        modelBuilder.Entity<AppointmentRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC27FE81B743");

            entity.ToTable("AppointmentRequest");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CancelReason)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CancelledDate).HasColumnType("datetime");
            entity.Property(e => e.ConsultantId).HasColumnName("ConsultantID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Duration).HasDefaultValue(60);
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.RequestedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Consultant).WithMany(p => p.AppointmentRequestConsultants)
                .HasForeignKey(d => d.ConsultantId)
                .HasConstraintName("FK__Appointme__Consu__52593CB8");

            entity.HasOne(d => d.Member).WithMany(p => p.AppointmentRequestMembers)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__Appointme__Membe__5165187F");
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlogPost__3214EC2718B65F99");

            entity.ToTable("BlogPost");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Image).HasColumnType("text");
            entity.Property(e => e.Tags)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.BlogPosts)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__BlogPost__Create__7D439ABD");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC2754549251");

            entity.ToTable("Comment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BlogPostId).HasColumnName("BlogPostID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.BlogPost).WithMany(p => p.Comments)
                .HasForeignKey(d => d.BlogPostId)
                .HasConstraintName("FK__Comment__BlogPos__00200768");

            entity.HasOne(d => d.Member).WithMany(p => p.Comments)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__Comment__MemberI__01142BA1");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__Comment__ParentC__02084FDA");
        });

        modelBuilder.Entity<CommunicationActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Communic__3214EC27DD6E360F");

            entity.ToTable("CommunicationActivity");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.CommunicationActivities)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Communica__Creat__76969D2E");
        });

        modelBuilder.Entity<ConsultantSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Consulta__3214EC27659EAFE1");

            entity.ToTable("ConsultantSchedule");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ConsultantId).HasColumnName("ConsultantID");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            entity.HasOne(d => d.Consultant).WithMany(p => p.ConsultantSchedules)
                .HasForeignKey(d => d.ConsultantId)
                .HasConstraintName("FK__Consultan__Consu__4D94879B");
        });

        modelBuilder.Entity<ConsultationNote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Consulta__3214EC27F92A35F1");

            entity.ToTable("ConsultationNote");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.ConsultantId).HasColumnName("ConsultantID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Notes).HasColumnType("text");

            entity.HasOne(d => d.Appointment).WithMany(p => p.ConsultationNotes)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK__Consultat__Appoi__571DF1D5");

            entity.HasOne(d => d.Consultant).WithMany(p => p.ConsultationNoteConsultants)
                .HasForeignKey(d => d.ConsultantId)
                .HasConstraintName("FK__Consultat__Consu__5812160E");

            entity.HasOne(d => d.Member).WithMany(p => p.ConsultationNoteMembers)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__Consultat__Membe__59063A47");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC27CB2ED758");

            entity.ToTable("Course");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Image)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Level)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PassingScore).HasDefaultValue(70);
            entity.Property(e => e.QuizQuestions).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Course__CreatedB__5DCAEF64");
        });

        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEn__3214EC27F5646850");

            entity.ToTable("CourseEnrollment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Enrolled");
            entity.Property(e => e.SubmissionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__CourseEnr__Cours__60A75C0F");

            entity.HasOne(d => d.Member).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__CourseEnr__Membe__619B8048");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC277D4ED84F");

            entity.ToTable("Notification");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.SendDate).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__04E4BC85");
        });

        modelBuilder.Entity<QuizSubmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__QuizSubm__3214EC27266FF610");

            entity.ToTable("QuizSubmission");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.SubmissionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.QuizSubmissions)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__QuizSubmi__Cours__656C112C");

            entity.HasOne(d => d.Member).WithMany(p => p.QuizSubmissions)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__QuizSubmi__Membe__66603565");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Survey__3214EC2711A38236");

            entity.ToTable("Survey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Questions).HasColumnType("text");
            entity.Property(e => e.ScoringRules).HasColumnType("text");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Survey__CreatedB__693CA210");
        });

        modelBuilder.Entity<SurveySubmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SurveySu__3214EC270CF03888");

            entity.ToTable("SurveySubmission");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Answers).HasColumnType("text");
            entity.Property(e => e.IsAnonymous).HasDefaultValue(false);
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.RiskLevel)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubmissionDate).HasColumnType("datetime");
            entity.Property(e => e.SurveyId).HasColumnName("SurveyID");

            entity.HasOne(d => d.Member).WithMany(p => p.SurveySubmissions)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__SurveySub__Membe__6D0D32F4");

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveySubmissions)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK__SurveySub__Surve__6C190EBB");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC276A98FD1A");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534F7E10660").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AgeGroup)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmailVerified).HasDefaultValue(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ProfileData).HasColumnType("text");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserInquiry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserInqu__3214EC27697AF84F");

            entity.ToTable("UserInquiry");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AssignedToId).HasColumnName("AssignedToID");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Open");
            entity.Property(e => e.Subject)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedTo).WithMany(p => p.UserInquiryAssignedTos)
                .HasForeignKey(d => d.AssignedToId)
                .HasConstraintName("FK__UserInqui__Assig__08B54D69");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.UserInquiryCreatedBies)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__UserInqui__Creat__07C12930");
        });

        modelBuilder.Entity<UserInquiryMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserInqu__3214EC276D490F3E");

            entity.ToTable("UserInquiryMessage");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.InquiryId).HasColumnName("InquiryID");
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.SenderId).HasColumnName("SenderID");
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.Inquiry).WithMany(p => p.UserInquiryMessages)
                .HasForeignKey(d => d.InquiryId)
                .HasConstraintName("FK__UserInqui__Inqui__0C85DE4D");

            entity.HasOne(d => d.Sender).WithMany(p => p.UserInquiryMessages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__UserInqui__Sende__0D7A0286");
        });

        modelBuilder.Entity<UserSurvey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSurv__3214EC273876A689");

            entity.ToTable("UserSurvey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RoleInSurvey)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.SurveyId).HasColumnName("SurveyID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Submission).WithMany(p => p.UserSurveys)
                .HasForeignKey(d => d.SubmissionId)
                .HasConstraintName("FK__UserSurve__Submi__72C60C4A");

            entity.HasOne(d => d.Survey).WithMany(p => p.UserSurveys)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK__UserSurve__Surve__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.UserSurveys)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserSurve__UserI__70DDC3D8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
