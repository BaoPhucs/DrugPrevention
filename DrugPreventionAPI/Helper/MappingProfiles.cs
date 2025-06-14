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
        }
    }
}
