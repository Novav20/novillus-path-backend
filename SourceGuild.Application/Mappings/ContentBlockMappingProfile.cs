using SourceGuild.Application.DTOs.ContentBlock;
using SourceGuild.Domain.Entities.Content;

namespace SourceGuild.Application.Mappings;

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
