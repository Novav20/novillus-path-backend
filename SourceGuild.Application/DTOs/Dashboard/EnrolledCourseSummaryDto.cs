namespace SourceGuild.Application.DTOs.Dashboard;

public record EnrolledCourseSummaryDto
{
    public Guid CourseId { get; init; }
    public required string Title { get; init; }
    public string? ImageUrl { get; init; }
    public required string InstructorName { get; init; }
    public int ProgressPercentage { get; init; } // Placeholder for now
}
