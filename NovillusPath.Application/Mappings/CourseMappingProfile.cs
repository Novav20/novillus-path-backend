using AutoMapper;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Mappings;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections != null ? src.Sections.OrderBy(s => s.Order).ToList() : null));

        CreateMap<CreateCourseDto, Course>();

        CreateMap<UpdateCourseDto, Course>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
