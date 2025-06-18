using AutoMapper;
using NovillusPath.Application.DTOs.Section;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Mappings;

public class SectionMappingProfile : Profile
{
    public SectionMappingProfile()
    {
        CreateMap<Section, SectionDto>();
        CreateMap<CreateSectionDto,Section>();
        CreateMap<UpdateSectionDto, Section>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
