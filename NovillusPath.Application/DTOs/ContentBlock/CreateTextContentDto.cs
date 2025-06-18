using NovillusPath.Domain.Enums;

namespace NovillusPath.Application.DTOs.ContentBlock;

public class CreateTextContentDto : CreateContentBlockBaseDto
{
    public required string Text { get; set; }

    public CreateTextContentDto() => Type = ContentBlockType.Text;
}
