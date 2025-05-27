using System;
using AutoMapper;
using NovillusPath.Application.DTOs.Course;
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
    }
}