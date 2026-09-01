namespace SourceGuild.Application.DTOs.Course;

public record UpdateCourseStatusDto
{
    public required string NewStatus { get; init; }
}