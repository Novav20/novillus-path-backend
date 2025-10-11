using NovillusPath.Application.DTOs.Dashboard;

namespace NovillusPath.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<StudentDashboardDto> GetStudentDashboardAsync(CancellationToken cancellationToken);
    Task<InstructorDashboardDto> GetInstructorDashboardAsync(CancellationToken cancellationToken);
}
