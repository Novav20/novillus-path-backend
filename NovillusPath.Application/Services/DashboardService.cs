using NovillusPath.Application.DTOs.Dashboard;

namespace NovillusPath.Application.Services;

public class DashboardService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<StudentDashboardDto> GetStudentDashboardAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new ServiceAuthorizationException("User ID could not be determined.");

        var enrollments = await _unitOfWork.EnrollmentRepository.GetEnrollmentsByUserIdAsync(userId, true, cancellationToken);

        var enrolledCoursesSummary = enrollments
            .Where(e => e.Course != null)
            .Select(e => _mapper.Map<EnrolledCourseSummaryDto>(e))
            .ToList();

        return new StudentDashboardDto
        {
            EnrolledCourses = enrolledCoursesSummary
        };
    }

    public async Task<InstructorDashboardDto> GetInstructorDashboardAsync(CancellationToken cancellationToken)
    {
        var instructorId = _currentUserService.UserId ?? throw new ServiceAuthorizationException("User ID could not be determined.");

        var courseSummaries = await _unitOfWork.CourseRepository.GetInstructorCoursesSummaryAsync(instructorId, cancellationToken);

        var totalEnrollments = courseSummaries.Sum(c => c.StudentCount);
        var coursesWithRatings = courseSummaries.Where(c => c.AverageRating > 0).ToList();
        var overallAverageRating = coursesWithRatings.Any() ? coursesWithRatings.Average(c => c.AverageRating) : 0;

        return new InstructorDashboardDto
        {
            TotalCourses = courseSummaries.Count,
            TotalEnrollments = totalEnrollments,
            OverallAverageRating = overallAverageRating,
            Courses = [.. courseSummaries]
        };
    }
}
