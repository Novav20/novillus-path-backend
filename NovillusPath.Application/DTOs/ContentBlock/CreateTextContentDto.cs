namespace NovillusPath.Application.DTOs.ContentBlock;

public record CreateTextContentDto : CreateContentBlockBaseDto
{
    public required string Text { get; init; }

    public CreateTextContentDto() => Type = ContentBlockType.Text;
}
