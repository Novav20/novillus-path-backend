using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Dashboard;
using NovillusPath.Application.Exceptions;
using NovillusPath.Application.Interfaces.Services;

namespace NovillusPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IDashboardService dashboardService) : BaseApiController
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        [HttpGet("student")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<StudentDashboardDto>> GetStudentDashboard(CancellationToken cancellationToken)
        {
            var dashboard = await _dashboardService.GetStudentDashboardAsync(cancellationToken);
            return Ok(dashboard);
        }

        [HttpGet("instructor")]
        [Authorize(Roles = "Instructor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InstructorDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<InstructorDashboardDto>> GetInstructorDashboard(CancellationToken cancellationToken)
        {
            var dashboard = await _dashboardService.GetInstructorDashboardAsync(cancellationToken);
            return Ok(dashboard);
        }
    }
}
