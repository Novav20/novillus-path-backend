using SourceGuild.Application.DTOs.Dashboard;

namespace SourceGuild.Application.Mappings;

public class DashboardMappingProfile : Profile
{
    public DashboardMappingProfile()
    {
        CreateMap<Enrollment, EnrolledCourseSummaryDto>()
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Course.Title))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Course.ImageUrl))
            .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor != null ? src.Course.Instructor.FullName : "N/A"))
            .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => 0)); // Placeholder
    }
}
