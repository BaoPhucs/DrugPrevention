using AutoMapper;
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
                .ForMember(dest => dest.ReviewComments, opt => opt.Ignore());
            CreateMap<CourseMaterial, CourseMaterialDTO>();
            CreateMap<CourseMaterialDTO, CourseMaterial>();
            CreateMap<CourseMaterial, CourseMaterialReadDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<CourseEnrollment, CourseEnrollmentDTO>();

            // 1) QuizSubmissionAnswer → QuizAnswerDTO
            CreateMap<QuizSubmissionAnswer, QuizAnswerDTO>()
                // QuizSubmissionAnswer.QuestionId là int? nên phải null-coalesce
                .ForMember(d => d.QuestionId, o => o.MapFrom(s => s.QuestionId ?? 0))
                .ForMember(d => d.OptionId, o => o.MapFrom(s => s.OptionId ?? 0));

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

            //7. Tag, TagDTO, BlogPost - DTO
            // Tag
            CreateMap<Tag, TagDTO>();
            CreateMap<CreateTagDTO, Tag>();

            // BlogPost → DTO
            CreateMap<BlogPost, BlogPostDTO>()
                .ForMember(d => d.Tags,
                           o => o.MapFrom(s => s.BlogTags.Select(bt => bt.Tag)));

            // DTO → BlogPost
            CreateMap<CreateBlogPostDTO, BlogPost>()
                .ForMember(d => d.BlogTags, opt => opt.Ignore())
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.UpdatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateBlogPostDTO, BlogPost>()
                .ForMember(d => d.BlogTags, opt => opt.Ignore())
                .ForMember(d => d.UpdatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));


        }
    }
}
//trial commit