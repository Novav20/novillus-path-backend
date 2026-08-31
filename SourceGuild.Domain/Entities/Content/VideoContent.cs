namespace SourceGuild.Domain.Entities.Content;

public class VideoContent : ContentBlock
{
    public required string VideoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Transcription { get; set; }
    public int DurationMinutes { get; set; }
}
