namespace NovillusPath.Application.DTOs.ContentBlock;

public class VideoContentDto : ContentBlockDto
{
    public required string VideoUrl { get; init; }
    public string? ThumbnailUrl { get; init; }
    public string? Transcription { get; init; }
    public int DurationMinutes { get; init; }

    public VideoContentDto() => Type = ContentBlockType.Video;
}
