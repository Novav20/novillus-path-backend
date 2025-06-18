using AutoMapper;
using NovillusPath.Application.DTOs.ContentBlock;
using NovillusPath.Domain.Entities.Content;

namespace NovillusPath.Application.Mappings;

public class ContentBlockMappingProfile : Profile
{
    public ContentBlockMappingProfile()
    {
        CreateMap<ContentBlock, ContentBlockDto>()
            .Include<TextContent, TextContentDto>()
            .Include<VideoContent, VideoContentDto>();

        CreateMap<TextContent, TextContentDto>();
        CreateMap<VideoContent, VideoContentDto>();

        CreateMap<CreateTextContentDto, TextContent>();
        CreateMap<CreateVideoContentDto, VideoContent>();
    }
}
