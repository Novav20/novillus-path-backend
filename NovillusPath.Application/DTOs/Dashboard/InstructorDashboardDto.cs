namespace NovillusPath.Application.DTOs.Dashboard;

public class InstructorDashboardDto
{
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public double OverallAverageRating { get; set; }
    public List<CreatedCourseSummaryDto> Courses { get; set; } = [];
}
