using SourceGuild.Application.DTOs.Section;

namespace SourceGuild.Application.Mappings;

public class SectionMappingProfile : Profile
{
    public SectionMappingProfile()
    {
        CreateMap<Section, SectionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.Lessons != null ? src.Lessons.OrderBy(l => l.Order).ToList() : null));

        CreateMap<CreateSectionDto, Section>();
        CreateMap<UpdateSectionDto, Section>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
