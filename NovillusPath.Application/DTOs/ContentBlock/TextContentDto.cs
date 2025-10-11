namespace NovillusPath.Application.DTOs.ContentBlock;

public class TextContentDto : ContentBlockDto
{
    public required string Text { get; init; }
    public TextContentDto() => Type = ContentBlockType.Text;
}
