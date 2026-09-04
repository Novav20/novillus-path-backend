using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SourceGuild.Application.Constants;
using SourceGuild.Application.DTOs.Dashboard;
using SourceGuild.Application.Features.Dashboard;

namespace SourceGuild.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController(DashboardQueries dashboardQueries) : ControllerBase
{
    [HttpGet("student")]
    [Authorize(Roles = Roles.Student)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDashboardDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StudentDashboardDto>> GetStudentDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await dashboardQueries.GetStudentDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }

    [HttpGet("instructor")]
    [Authorize(Roles = Roles.Instructor)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InstructorDashboardDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InstructorDashboardDto>> GetInstructorDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await dashboardQueries.GetInstructorDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }
}