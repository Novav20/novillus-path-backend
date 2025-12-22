namespace SourceGuild.Application.DTOs.Lesson;

public record UpdateLessonStatusDto
{
    public required string Status { get; init; }
}
