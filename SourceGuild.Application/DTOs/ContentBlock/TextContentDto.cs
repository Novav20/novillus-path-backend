namespace SourceGuild.Application.DTOs.ContentBlock;

public record TextContentDto : ContentBlockDto
{
    public required string Text { get; init; }
    public TextContentDto() => Type = ContentBlockType.Text;
}
