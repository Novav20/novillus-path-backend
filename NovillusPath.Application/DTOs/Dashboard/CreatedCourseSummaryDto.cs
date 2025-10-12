namespace NovillusPath.Application.DTOs.Dashboard;

public record CreatedCourseSummaryDto
{
    public Guid CourseId { get; init; }
    public required string Title { get; init; }
    public required string Status { get; init; }
    public int StudentCount { get; init; }
    public double AverageRating { get; init; }
}
