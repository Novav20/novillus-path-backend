using SourceGuild.Application.DTOs.Course;

namespace SourceGuild.Application.Mappings;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Count != 0 ? src.Reviews.Average(r => r.Rating) : 0.0))
            .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src => src.Reviews.Count != 0 ? src.Reviews.Count : 0))
            .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections != null ? src.Sections.OrderBy(s => s.Order).ToList() : null));

        CreateMap<CreateCourseDto, Course>();

        CreateMap<UpdateCourseDto, Course>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<CourseListProjectionDto, CourseDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));
    }
}
