namespace SourceGuild.Application.DTOs.Dashboard;

public record StudentDashboardDto
{
    public List<EnrolledCourseSummaryDto> EnrolledCourses { get; init; } = [];
    // Other dashboard elements can be added here in the future
}
