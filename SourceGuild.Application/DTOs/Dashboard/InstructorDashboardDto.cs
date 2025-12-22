namespace SourceGuild.Application.DTOs.Dashboard;

public record InstructorDashboardDto
{
    public int TotalCourses { get; init; }
    public int TotalEnrollments { get; init; }
    public double OverallAverageRating { get; init; }
    public List<CreatedCourseSummaryDto> Courses { get; init; } = [];
}
