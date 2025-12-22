using SourceGuild.Application.DTOs.Lesson;

namespace SourceGuild.Application.Mappings;

public class LessonMappingProfile : Profile
{
    public LessonMappingProfile()
    {
        CreateMap<Lesson, LessonDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ContentBlocks, opt => opt.MapFrom(src => src.ContentBlocks.OrderBy(cb => cb.Order)));

        CreateMap<CreateLessonDto, Lesson>()
            .ForMember(dest => dest.ContentBlocks, opt => opt.Ignore());

        CreateMap<UpdateLessonDto, Lesson>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
