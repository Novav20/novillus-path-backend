using NovillusPath.Domain.Enums;

namespace NovillusPath.Application.DTOs.ContentBlock;

public class CreateVideoContentDto : CreateContentBlockBaseDto
{
    public required string VideoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Transcription { get; set; }
    public int DurationMinutes { get; set; }
    public CreateVideoContentDto() => Type = ContentBlockType.Video;
}
