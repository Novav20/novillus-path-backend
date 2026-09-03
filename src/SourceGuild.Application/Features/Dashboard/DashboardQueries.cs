using SourceGuild.Application.Common.Mappings;
using SourceGuild.Application.DTOs.Dashboard;
using SourceGuild.Application.Interfaces.Common;
using SourceGuild.Application.Interfaces.Persistence;

namespace SourceGuild.Application.Features.Dashboard;

public class DashboardQueries(
    ICourseRepository courseRepository,
    IEnrollmentRepository enrollmentRepository,
    ICurrentUserService currentUserService)
{
    public async Task<StudentDashboardDto> GetStudentDashboardAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUserService.UserId.HasValue || currentUserService.UserId.Value == Guid.Empty) 
            return new StudentDashboardDto();

        var userId = currentUserService.UserId.Value;

        var enrollments = await enrollmentRepository.GetByUserIdAsync(userId, includeCourseDetails: true, cancellationToken);
        return new StudentDashboardDto
        {
            EnrolledCourses = enrollments.Select(e => e.ToEnrolledSummaryDto()).ToList()
        };
    }

    public async Task<InstructorDashboardDto> GetInstructorDashboardAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUserService.UserId.HasValue || currentUserService.UserId.Value == Guid.Empty) 
            return new InstructorDashboardDto();

        var instructorId = currentUserService.UserId.Value;

        var coursesSummary = await courseRepository.GetInstructorCoursesSummaryAsync(instructorId, cancellationToken);
        
        return new InstructorDashboardDto
        {
            TotalCourses = coursesSummary.Count,
            TotalEnrollments = coursesSummary.Sum(c => c.StudentCount),
            OverallAverageRating = coursesSummary.Count > 0 ? coursesSummary.Average(c => c.AverageRating) : 0,
            Courses = coursesSummary.ToList()
        };
    }
}