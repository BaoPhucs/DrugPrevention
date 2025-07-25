﻿using AutoMapper;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, RegisterUserDTO>();
            CreateMap<RegisterUserDTO, User>();
            CreateMap<User, UserDTO>();
            CreateMap<Course, CourseDTO>();
            CreateMap<CourseDTO, Course>()
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewComments, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.WorkflowState, opt => opt.Ignore());
            CreateMap<CourseMaterial, CourseMaterialDTO>();
            CreateMap<CourseMaterialDTO, CourseMaterial>();
            CreateMap<CourseMaterial, CourseMaterialReadDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<CourseEnrollment, CourseEnrollmentDTO>();
            // Entity → DTO
            CreateMap<BlogPost, BlogPostDTO>()
                // if your DTO has a plain Tags collection, map via the join table:
                .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(src => src.BlogTags.Select(bt => bt.Tag)));

            // (Optionally, if you want two-way mapping)
            CreateMap<BlogPostDTO, BlogPost>();
            CreateMap<Tag, TagDTO>();
            CreateMap<CreateTagDTO, Tag>();
            // incoming POST
            CreateMap<CreateBlogPostDTO, BlogPost>()
                // ignore ID (won’t be set by the client)
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // incoming PUT
            CreateMap<UpdateBlogPostDTO, BlogPost>()
                // the repo already loads the existing entity (so you don’t overwrite the PK)
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // 1) QuizSubmissionAnswer → QuizAnswerDTO
            CreateMap<QuizSubmissionAnswer, QuizAnswerDTO>()
                // QuizSubmissionAnswer.QuestionId là int? nên phải null-coalesce
                .ForMember(d => d.QuestionId, o => o.MapFrom(s => s.QuestionId ?? 0))
                .ForMember(d => d.OptionId, o => o.MapFrom(s => s.OptionId ?? 0))
                .ForMember(d => d.ScoreValue, o => o.MapFrom(src => src.Option.ScoreValue));

            // 2) QuizSubmission → QuizSubmissionReadDTO
            CreateMap<QuizSubmission, QuizSubmissionReadDTO>();

            // 3) QuizSubmission → QuizSubmissionDetailDTO, include Answers
            CreateMap<QuizSubmission, QuizSubmissionDetailDTO>()
                .ForMember(d => d.Answers,
                           o => o.MapFrom(s => s.QuizSubmissionAnswers));

            // 4) QuestionBank → QuizQuestionDTO
            CreateMap<QuestionBank, QuizQuestionDTO>()
                .ForMember(d => d.QuestionId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.QuestionText, o => o.MapFrom(s => s.QuestionText));

            // 5) QuestionOption → QuizOptionDTO
            CreateMap<QuestionOption, QuizOptionDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.OptionText, o => o.MapFrom(s => s.OptionText));


            //  Entity -> DTO
            CreateMap<QuestionBank, QuestionDTO>()
                .ForMember(d => d.QuestionText, o => o.MapFrom(s => s.QuestionText))
                .ForMember(d => d.Level, o => o.MapFrom(s => s.Level))
                .ForMember(d => d.Options, o => o.MapFrom(s => s.QuestionOptions));

            CreateMap<QuestionOption, OptionDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.OptionText, o => o.MapFrom(s => s.OptionText));

            // CreateDTO -> Entity
            CreateMap<CreateQuestionDTO, QuestionBank>();
            CreateMap<CreateOptionDTO, QuestionOption>();

            // SurveyAdd commentMore actions
            CreateMap<Survey, SurveyDTO>();
            CreateMap<CreateSurveyDTO, Survey>();

            // SurveyQuestion
            CreateMap<SurveyQuestion, SurveyQuestionDTO>()
                .ForMember(dest => dest.Options,
                           opt => opt.MapFrom(src => src.SurveyOptions.OrderBy(o => o.Sequence)));
            CreateMap<CreateSurveyQuestionDTO, SurveyQuestion>();

            // SurveyOption
            CreateMap<SurveyOption, SurveyOptionDTO>();
            CreateMap<CreateSurveyOptionDTO, SurveyOption>();

            // Submission
            CreateMap<SurveySubmission, SurveySubmissionReadDTO>();
            CreateMap<SurveySubmission, SurveySubmissionDetailDTO>()
                .ForMember(d => d.Answers,
                           o => o.MapFrom(src => src.SurveySubmissionAnswers
                                                   .Select(a => new SurveyAnswerDTO
                                                   {
                                                       QuestionId = a.QuestionId,
                                                       OptionId = a.OptionId
                                                   })));

            //Inquiry <-> InquiryAssignment
            CreateMap<InquiryAssignment, InquiryAssignmentDTO>();
            CreateMap<CreateInquiryAssignment, InquiryAssignment>()
                .ForMember(d => d.AssignedDate,
                           o => o.MapFrom(s => s.AssignedDate ?? DateTime.UtcNow));

            CreateMap<InquiryComment, InquiryCommentDTO>();
            CreateMap<CreateInquiryCommentDTO, InquiryComment>()
                .ForMember(dest => dest.CreatedDate,
                           opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UserInquiry, UserInquiryDTO>();
            CreateMap<CreateUserInquiryDTO, UserInquiry>()
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.LastUpdated, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<CreateUserInquiryDTO, UserInquiry>()
                .ForMember(d => d.LastUpdated, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UpdateUserInquiryDTO, UserInquiry>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Không cập nhật Id
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore()) // Không cập nhật CreatedById
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()); // Không cập nhật CreatedDate

            // Map entity -> DTO
            CreateMap<Comment, CommentDTO>();

            CreateMap<ConsultationNote, ConsultationNoteDTO>();
            // Map create-DTO -> entity
            CreateMap<CreateBlogPostCommentDTO, Comment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateCommentDTO, Comment>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateBlogPostDTO, BlogPost>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
    .ForMember(dest => dest.Title, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Title)))
    .ForMember(dest => dest.Content, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Content)))
    .ForMember(dest => dest.CoverImageUrl, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.CoverImageUrl)))
    .AfterMap((src, dest) =>
    {
        if (src.TagIds != null && src.TagIds.Any() && !src.TagIds.All(id => id == 0))
        {
            var oldTags = dest.BlogTags?.ToList() ?? new List<BlogTag>();
            foreach (var tag in oldTags)
            {
                dest.BlogTags.Remove(tag);
            }
            dest.BlogTags = src.TagIds
                .Where(tagId => tagId != 0)
                .Select(tagId => new BlogTag { BlogPostId = dest.Id, TagId = tagId })
                .ToList();
        }
        dest.UpdatedDate = DateTime.UtcNow;
    });

            CreateMap<CreateCommunicationActivityDTO, CommunicationActivity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Giữ nguyên Status mặc định
                .ForMember(dest => dest.Title, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Title)))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Description)))
                .ForMember(dest => dest.EventDate, opt => opt.Condition(src => src.EventDate != default))
                .ForMember(dest => dest.Location, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Location)))
                .ForMember(dest => dest.Capacity, opt => opt.Condition(src => src.Capacity.HasValue && src.Capacity > 0));

            //
            CreateMap<ConsultantSchedule, ConsultantScheduleDTO>();
            CreateMap<AppointmentRequest, AppointmentRequestDTO>();

            // CommunicationAct - ActParticipation
            CreateMap<CommunicationActivity, CommunicationActivityDTO>();
            CreateMap<CreateCommunicationActivityDTO, CommunicationActivity>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ActivityParticipation, ActivityParticipationDTO>();
            CreateMap<ActivityParticipationDTO, ActivityParticipation>();


            CreateMap<Certificate, CertificateDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.MemberId))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.IssuedDate, opt => opt.MapFrom(src => src.IssuedDate))
            .ForMember(dest => dest.CertificateNo, opt => opt.MapFrom(src => src.CertificateNo))
            .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl));

            // Nếu cần ánh xạ ngược (tùy chọn)
            CreateMap<CertificateDTO, Certificate>();
        }
    }
}
