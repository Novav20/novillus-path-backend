namespace NovillusPath.API.Controllers
{
    /// <summary>
    /// API controller for retrieving dashboard information for students and instructors.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IDashboardService dashboardService) : BaseApiController
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        /// <summary>
        /// Retrieves the dashboard summary for the currently authenticated student.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A StudentDashboardDto containing enrolled courses and progress.</returns>
        [HttpGet("student")]
        [Authorize(Roles = Roles.Student)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<StudentDashboardDto>> GetStudentDashboard(CancellationToken cancellationToken)
        {
            var dashboard = await _dashboardService.GetStudentDashboardAsync(cancellationToken);
            return Ok(dashboard);
        }

        /// <summary>
        /// Retrieves the dashboard summary for the currently authenticated instructor.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An InstructorDashboardDto containing created courses and statistics.</returns>
        [HttpGet("instructor")]
        [Authorize(Roles = Roles.Instructor)]
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
