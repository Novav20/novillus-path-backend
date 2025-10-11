namespace NovillusPath.Application.DTOs.Dashboard;

public class EnrolledCourseSummaryDto
{
    public Guid CourseId { get; set; }
    public required string Title { get; set; }
    public string? ImageUrl { get; set; }
    public required string InstructorName { get; set; }
    public int ProgressPercentage { get; set; } // Placeholder for now
}
