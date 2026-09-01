using SourceGuild.Domain.Common;

namespace SourceGuild.Domain.Entities.Content;

public class VideoContent : ContentBlock
{
    private VideoContent() { }

    internal static Result<VideoContent> Create(
        Guid lessonId, 
        string videoUrl, 
        int durationMinutes, 
        int order, 
        string? thumbnailUrl = null, 
        string? transcription = null)
    {
        if (string.IsNullOrWhiteSpace(videoUrl))
            return Result<VideoContent>.Failure(Error.Validation("VideoContent.EmptyUrl", "La URL del video es obligatoria."));

        if (durationMinutes < 0)
            return Result<VideoContent>.Failure(Error.Validation("VideoContent.InvalidDuration", "La duración del video no puede ser negativa."));

        var content = new VideoContent
        {
            Id = Guid.NewGuid(),
            LessonId = lessonId,
            VideoUrl = videoUrl.Trim(),
            DurationMinutes = durationMinutes,
            ThumbnailUrl = thumbnailUrl?.Trim(),
            Transcription = transcription?.Trim(),
            Order = order,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return Result<VideoContent>.Success(content);
    }

    public string VideoUrl { get; private set; } = string.Empty;
    public string? ThumbnailUrl { get; private set; }
    public string? Transcription { get; private set; }
    public int DurationMinutes { get; private set; }

    public Result UpdateDetails(string videoUrl, int durationMinutes, string? thumbnailUrl, string? transcription)
    {
        if (string.IsNullOrWhiteSpace(videoUrl))
            return Result.Failure(Error.Validation("VideoContent.EmptyUrl", "La URL del video es obligatoria."));

        if (durationMinutes < 0)
            return Result.Failure(Error.Validation("VideoContent.InvalidDuration", "La duración del video no puede ser negativa."));

        VideoUrl = videoUrl.Trim();
        DurationMinutes = durationMinutes;
        ThumbnailUrl = thumbnailUrl?.Trim();
        Transcription = transcription?.Trim();
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}