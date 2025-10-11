using AutoMapper;
using NovillusPath.Application.DTOs.Dashboard;
using NovillusPath.Application.Exceptions;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Application.Interfaces.Services;

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
}
