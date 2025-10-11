namespace NovillusPath.Application.DTOs.Dashboard;

public class CreatedCourseSummaryDto
{
    public Guid CourseId { get; set; }
    public required string Title { get; set; }
    public required string Status { get; set; }
    public int StudentCount { get; set; }
    public double AverageRating { get; set; }
}
