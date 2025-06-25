using System;
using System.Collections.Generic;
using DrugPreventionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DrugPreventionAPI.Data;

public partial class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityParticipation> ActivityParticipations { get; set; }

    public virtual DbSet<AppointmentRequest> AppointmentRequests { get; set; }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<BlogTag> BlogTags { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommunicationActivity> CommunicationActivities { get; set; }

    public virtual DbSet<ConsultantSchedule> ConsultantSchedules { get; set; }

    public virtual DbSet<ConsultationNote> ConsultationNotes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseEnrollment> CourseEnrollments { get; set; }

    public virtual DbSet<CourseQuestion> CourseQuestions { get; set; }

    public virtual DbSet<CourseMaterial> CourseMaterials { get; set; }

    public virtual DbSet<InquiryAssignment> InquiryAssignments { get; set; }

    public virtual DbSet<InquiryComment> InquiryComments { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<QuestionBank> QuestionBanks { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<QuizSubmission> QuizSubmissions { get; set; }

    public virtual DbSet<QuizSubmissionAnswer> QuizSubmissionAnswers { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<SurveyOption> SurveyOptions { get; set; }

    public virtual DbSet<SurveyQuestion> SurveyQuestions { get; set; }

    public virtual DbSet<SurveySubmission> SurveySubmissions { get; set; }

    public virtual DbSet<SurveySubmissionAnswer> SurveySubmissionAnswers { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserInquiry> UserInquiries { get; set; }

    public virtual DbSet<UserSurvey> UserSurveys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityParticipation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Activity__3214EC27C6EEF850");

            entity.ToTable("ActivityParticipation");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Registered");

            entity.HasOne(d => d.Activity).WithMany(p => p.ActivityParticipations)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_ActivityParticipation_Activity");

            entity.HasOne(d => d.Member).WithMany(p => p.ActivityParticipations)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_ActivityParticipation_Member");
        });

        modelBuilder.Entity<AppointmentRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC27D3D09ADD");

            entity.ToTable("AppointmentRequest");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CancelReason)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CancelledDate).HasColumnType("datetime");
            entity.Property(e => e.ConsultantId).HasColumnName("ConsultantID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Consultant).WithMany(p => p.AppointmentRequestConsultants)
                .HasForeignKey(d => d.ConsultantId)
                .HasConstraintName("FK_AppointmentRequest_Consultant");

            entity.HasOne(d => d.Member).WithMany(p => p.AppointmentRequestMembers)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_AppointmentRequest_Member");

            entity.HasOne(d => d.Schedule).WithMany(p => p.AppointmentRequests)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("FK_AppointmentRequest_Schedule");
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlogPost__3214EC27E5590608");

            entity.ToTable("BlogPost");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).IsUnicode(false);
            entity.Property(e => e.CoverImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CoverImageURL");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.BlogPosts)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_BlogPost_Author");

            //entity.HasMany(d => d.Tags).WithMany(p => p.BlogPosts)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "BlogTag",
            //        r => r.HasOne<Tag>().WithMany()
            //            .HasForeignKey("TagId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK_BlogTag_Tag"),
            //        l => l.HasOne<BlogPost>().WithMany()
            //            .HasForeignKey("BlogPostId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK_BlogTag_Post"),
            //        j =>
            //        {
            //            j.HasKey("BlogPostId", "TagId");
            //            j.ToTable("BlogTag");
            //            j.IndexerProperty<int>("BlogPostId").HasColumnName("BlogPostID");
            //            j.IndexerProperty<int>("TagId").HasColumnName("TagID");
            //        });
        });

        modelBuilder.Entity<BlogTag>(entity =>
        {
            entity.ToTable("BlogTag");
            // composite PK
            entity.HasKey(bt => new { bt.BlogPostId, bt.TagId });

            // FK → BlogPost
            entity.HasOne(bt => bt.BlogPost)
                  .WithMany(bp => bp.BlogTags)
                  .HasForeignKey(bt => bt.BlogPostId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_BlogTag_Post");

            // FK → Tag
            entity.HasOne(bt => bt.Tag)
                  .WithMany(t => t.BlogTags)
                  .HasForeignKey(bt => bt.TagId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_BlogTag_Tag");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Content).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("Visible");

            // Quan hệ với User (Member)
            entity.HasOne(e => e.Member)
                  .WithMany(u => u.Comments)
                  .HasForeignKey(e => e.MemberId)
                  .HasConstraintName("FK_Comment_Member");

            // Quan hệ đệ quy Parent → Replies
            entity.HasOne(e => e.ParentComment)
                  .WithMany(e => e.Replies)
                  .HasForeignKey(e => e.ParentCommentId)
                  .HasConstraintName("FK_Comment_Parent");

            // Quan hệ tới BlogPost
            entity.HasOne(e => e.BlogPost)
                  .WithMany(bp => bp.Comments)
                  .HasForeignKey(e => e.BlogPostId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_Comment_BlogPost");

            // Quan hệ tới CommunicationActivity
            entity.HasOne(e => e.Activity)
                  .WithMany(a => a.Comments)
                  .HasForeignKey(e => e.ActivityId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_Comment_Activity");
        });

        modelBuilder.Entity<CommunicationActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Communic__3214EC27173C6DCF");

            entity.ToTable("CommunicationActivity");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.Location)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.CommunicationActivities)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_CommunicationActivity_Creator");
        });

        modelBuilder.Entity<ConsultantSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Consulta__3214EC274CE11072");

            entity.ToTable("ConsultantSchedule");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ConsultantId).HasColumnName("ConsultantID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            entity.HasOne(d => d.Consultant).WithMany(p => p.ConsultantSchedules)
                .HasForeignKey(d => d.ConsultantId)
                .HasConstraintName("FK_ConsultantSchedule_User");
        });

        modelBuilder.Entity<ConsultationNote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Consulta__3214EC27A1738B3F");

            entity.ToTable("ConsultationNote");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.ConsultantId).HasColumnName("ConsultantID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Notes).IsUnicode(false);

            entity.HasOne(d => d.Appointment).WithMany(p => p.ConsultationNotes)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK_ConsultationNote_Appointment");

            entity.HasOne(d => d.Consultant).WithMany(p => p.ConsultationNoteConsultants)
                .HasForeignKey(d => d.ConsultantId)
                .HasConstraintName("FK_ConsultationNote_Consultant");

            entity.HasOne(d => d.Member).WithMany(p => p.ConsultationNoteMembers)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_ConsultationNote_Member");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC273ED51E76");

            entity.ToTable("Course");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Content).IsUnicode(false);
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Level)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PassingScore).HasDefaultValue(70);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ReviewComments)
                .HasMaxLength(int.MaxValue)
                .IsUnicode(false);
            entity.Property(e => e.UpdateById).HasColumnName("UpdateByID");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_Course_Creator");

            //entity.HasMany(d => d.Questions).WithMany(p => p.Courses)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "CourseQuestion",
            //        r => r.HasOne<QuestionBank>().WithMany()
            //            .HasForeignKey("QuestionId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK_CourseQuestion_Question"),
            //        l => l.HasOne<Course>().WithMany()
            //            .HasForeignKey("CourseId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK_CourseQuestion_Course"),
            //        j =>
            //        {
            //            j.HasKey("CourseId", "QuestionId");
            //            j.ToTable("CourseQuestion");
            //            j.IndexerProperty<int>("CourseId").HasColumnName("CourseID");
            //            j.IndexerProperty<int>("QuestionId").HasColumnName("QuestionID");
            //        });
        });


        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseEn__3214EC27B3F5A0D1");
            entity.ToTable("CourseEnrollment");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.ParticipationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Enrolled");
            entity.HasOne(d => d.Course).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_CourseEnrollment_Course");
            entity.HasOne(d => d.Member).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_CourseEnrollment_Member");
        });

        modelBuilder.Entity<CourseQuestion>(entity =>
        {
            entity.ToTable("CourseQuestion");
            entity.HasKey(cq => new { cq.CourseId, cq.QuestionId });

            entity.Property(cq => cq.CourseId)
                  .HasColumnName("CourseID");
            entity.Property(cq => cq.QuestionId)
                  .HasColumnName("QuestionID");

            entity.HasOne(cq => cq.Course)
                  .WithMany(c => c.CourseQuestions)
                  .HasForeignKey(cq => cq.CourseId)
                  .HasConstraintName("FK_CourseQuestion_Course");

            entity.HasOne(cq => cq.Question)
                  .WithMany(q => q.CourseQuestions)
                  .HasForeignKey(cq => cq.QuestionId)
                  .HasConstraintName("FK_CourseQuestion_Question");
        });

        modelBuilder.Entity<CourseMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseMa__3214EC27ACF8C0FC");

            entity.ToTable("CourseMaterial");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .IsUnicode(false)
                .HasColumnName("URL");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseMaterials)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_CourseMaterial_Course");
        });

        modelBuilder.Entity<InquiryAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InquiryA__3214EC27E8A4C774");

            entity.ToTable("InquiryAssignment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AssignedById).HasColumnName("AssignedByID");
            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.AssignedToId).HasColumnName("AssignedToID");
            entity.Property(e => e.InquiryId).HasColumnName("InquiryID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedBy).WithMany(p => p.InquiryAssignmentAssignedBies)
                .HasForeignKey(d => d.AssignedById)
                .HasConstraintName("FK_InquiryAssignment_By");

            entity.HasOne(d => d.AssignedTo).WithMany(p => p.InquiryAssignmentAssignedTos)
                .HasForeignKey(d => d.AssignedToId)
                .HasConstraintName("FK_InquiryAssignment_To");

            entity.HasOne(d => d.Inquiry).WithMany(p => p.InquiryAssignments)
                .HasForeignKey(d => d.InquiryId)
                .HasConstraintName("FK_InquiryAssignment_Inquiry");
        });

        modelBuilder.Entity<InquiryComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InquiryC__3214EC27E9C25829");

            entity.ToTable("InquiryComment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AttachmentType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AttachmentUrl)
                .IsUnicode(false)
                .HasColumnName("AttachmentURL");
            entity.Property(e => e.CommentById).HasColumnName("CommentByID");
            entity.Property(e => e.CommentText).IsUnicode(false);
            entity.Property(e => e.CommentType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.InquiryId).HasColumnName("InquiryID");

            entity.HasOne(d => d.CommentBy).WithMany(p => p.InquiryComments)
                .HasForeignKey(d => d.CommentById)
                .HasConstraintName("FK_InquiryComment_By");

            entity.HasOne(d => d.Inquiry).WithMany(p => p.InquiryComments)
                .HasForeignKey(d => d.InquiryId)
                .HasConstraintName("FK_InquiryComment_Inquiry");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC27F33EFE7F");

            entity.ToTable("Notification");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Message).IsUnicode(false);
            entity.Property(e => e.SendDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<QuestionBank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC27AF1DCD51");

            entity.ToTable("QuestionBank");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Level)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.QuestionText).IsUnicode(false);
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC27E9861141");

            entity.ToTable("QuestionOption");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OptionText)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionOptions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_QuestionOption_Question");
        });

        modelBuilder.Entity<QuestionOption>()
        .HasOne(o => o.Question)
        .WithMany(q => q.QuestionOptions)
        .HasForeignKey(o => o.QuestionId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizSubmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__QuizSubm__3214EC27922467CE");

            entity.ToTable("QuizSubmission");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.SubmissionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.QuizSubmissions)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_QuizSubmission_Course");

            entity.HasOne(d => d.Member).WithMany(p => p.QuizSubmissions)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_QuizSubmission_Member");
        });

        modelBuilder.Entity<QuizSubmissionAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__QuizSubm__3214EC27DC738DDA");

            entity.ToTable("QuizSubmissionAnswer");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OptionId).HasColumnName("OptionID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");

            entity.HasOne(d => d.Option).WithMany(p => p.QuizSubmissionAnswers)
                .HasForeignKey(d => d.OptionId)
                .HasConstraintName("FK_QSA_Option");

            entity.HasOne(d => d.Question).WithMany(p => p.QuizSubmissionAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_QSA_Question");

            entity.HasOne(d => d.Submission).WithMany(p => p.QuizSubmissionAnswers)
                .HasForeignKey(d => d.SubmissionId)
                .HasConstraintName("FK_QSA_Submission");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Survey__3214EC274D9D4BF3");

            entity.ToTable("Survey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_Survey_Creator");
        });

        modelBuilder.Entity<SurveyOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SurveyOp__3214EC279697063B");

            entity.ToTable("SurveyOption");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OptionText)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.SurveyOptions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_SurveyOption_Question");
        });

        modelBuilder.Entity<SurveyQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SurveyQu__3214EC2774CD1051");

            entity.ToTable("SurveyQuestion");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuestionText).IsUnicode(false);
            entity.Property(e => e.SurveyId).HasColumnName("SurveyID");

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveyQuestions)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK_SurveyQuestion_Survey");
        });

        modelBuilder.Entity<SurveySubmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SurveySu__3214EC273F53121C");

            entity.ToTable("SurveySubmission");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IsAnonymous).HasDefaultValue(false);
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.RiskLevel)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubmissionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SurveyId).HasColumnName("SurveyID");

            entity.HasOne(d => d.Member).WithMany(p => p.SurveySubmissions)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_SurveySubmission_Member");

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveySubmissions)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK_SurveySubmission_Survey");
        });

        modelBuilder.Entity<SurveySubmissionAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SurveySu__3214EC2760BC51C2");

            entity.ToTable("SurveySubmissionAnswer");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OptionId).HasColumnName("OptionID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");

            entity.HasOne(d => d.Option).WithMany(p => p.SurveySubmissionAnswers)
                .HasForeignKey(d => d.OptionId)
                .HasConstraintName("FK_SSA_Option");

            entity.HasOne(d => d.Question).WithMany(p => p.SurveySubmissionAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_SSA_Question");

            entity.HasOne(d => d.Submission).WithMany(p => p.SurveySubmissionAnswers)
                .HasForeignKey(d => d.SubmissionId)
                .HasConstraintName("FK_SSA_Submission");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tag__3214EC27415F3F10");

            entity.ToTable("Tag");

            entity.HasIndex(e => e.Name, "UQ__Tag__737584F67019ABFA").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC271A11D3C6");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D1053436AD7F09").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AgeGroup)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
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
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProfileData).IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserInquiry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserInqu__3214EC27D3340BD8");

            entity.ToTable("UserInquiry");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Open");
            entity.Property(e => e.Subject)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.UserInquiries)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_UserInquiry_Creator");
        });

        modelBuilder.Entity<UserSurvey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSurv__3214EC27CB3EA0A1");

            entity.ToTable("UserSurvey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleInSurvey)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.SurveyId).HasColumnName("SurveyID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Submission).WithMany(p => p.UserSurveys)
                .HasForeignKey(d => d.SubmissionId)
                .HasConstraintName("FK_UserSurvey_Submission");

            entity.HasOne(d => d.Survey).WithMany(p => p.UserSurveys)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK_UserSurvey_Survey");

            entity.HasOne(d => d.User).WithMany(p => p.UserSurveys)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserSurvey_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
