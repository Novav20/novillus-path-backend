using SourceGuild.Application.DTOs.Dashboard;

namespace SourceGuild.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<StudentDashboardDto> GetStudentDashboardAsync(CancellationToken cancellationToken);
    Task<InstructorDashboardDto> GetInstructorDashboardAsync(CancellationToken cancellationToken);
}
