namespace NovillusPath.API.Controllers
{
    /// <summary>
    /// API controller for managing course enrollments.
    /// </summary>
    [Route("api/courses/{courseId}")]
    [ApiController]
    [Produces("application/json")]
    public class EnrollmentsController(IEnrollmentService enrollmentService, ICurrentUserService currentUserService) : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService = enrollmentService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        /// <summary>
        /// Enrolls the current user in a specified course.
        /// </summary>
        /// <param name="courseId">The ID of the course to enroll in.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("enroll")]
        [Authorize(Roles = Roles.Student + "," + Roles.Admin)] 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
        [ProducesResponseType(StatusCodes.Status403Forbidden)]  
        [ProducesResponseType(StatusCodes.Status404NotFound)]  
        public async Task<IActionResult> Enroll([FromRoute] Guid courseId, CancellationToken cancellationToken = default)
        {
            var userIdToEnroll = _currentUserService.UserId;

            if (!userIdToEnroll.HasValue)
            {
                // This case should ideally not be hit if [Authorize] and ICurrentUserService are working correctly.
                // It implies an authenticated user somehow doesn't have a UserId claim.
                return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "User ID could not be determined.", Status = StatusCodes.Status401Unauthorized });
            }

            // The service method will use its own ICurrentUserService to validate if the
            // authenticated user (represented by _currentUserService in the service) can
            // enroll the target userIdToEnroll.Value.
            await _enrollmentService.EnrollAsync(courseId, userIdToEnroll.Value, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Unenrolls the current user from a specified course.
        /// </summary>
        /// <param name="courseId">The ID of the course to unenroll from.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpDelete("unenroll")]
        [Authorize(Roles = Roles.Student + "," + Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unenroll(
    [FromRoute] Guid courseId,
    CancellationToken cancellationToken = default)
        {
            var userIdToUnenroll = _currentUserService.UserId;

            if (!userIdToUnenroll.HasValue)
            {
                return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "User ID could not be determined.", Status = StatusCodes.Status401Unauthorized });
            }

            await _enrollmentService.UnenrollAsync(courseId, userIdToUnenroll.Value, cancellationToken);
            return NoContent();
        }
    }
}