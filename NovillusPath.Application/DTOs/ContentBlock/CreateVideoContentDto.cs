namespace NovillusPath.Application.DTOs.ContentBlock;

public record CreateVideoContentDto : CreateContentBlockBaseDto
{
    public required string VideoUrl { get; init; }
    public string? ThumbnailUrl { get; init; }
    public string? Transcription { get; init; }
    public int DurationMinutes { get; init; }
    public CreateVideoContentDto() => Type = ContentBlockType.Video;
}
