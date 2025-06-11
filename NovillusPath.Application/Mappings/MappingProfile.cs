using AutoMapper;
using NovillusPath.Application.DTOs.Category;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.DTOs.Section;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateCourseDto, Course>();

        CreateMap<UpdateCourseDto, Course>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Section, SectionDto>();
        CreateMap<CreateSectionDto,Section>();
        CreateMap<UpdateSectionDto, Section>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}