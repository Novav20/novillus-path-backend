using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.Exceptions;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Services;

namespace NovillusPath.API.Controllers
{
    [Route("api/courses/{courseId}")]
    [ApiController]
    [Produces("application/json")]
    public class EnrollmentsController(IEnrollmentService enrollmentService, ICurrentUserService currentUserService) : BaseApiController()
    {
        private readonly IEnrollmentService _enrollmentService = enrollmentService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        [HttpPost("enroll")]
        [Authorize(Roles = "Student,Admin")] 
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

            try
            {
                // The service method will use its own ICurrentUserService to validate if the
                // authenticated user (represented by _currentUserService in the service) can
                // enroll the target userIdToEnroll.Value.
                await _enrollmentService.EnrollAsync(courseId, userIdToEnroll.Value, cancellationToken);
                return NoContent();
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFoundProblem(ex);
            }
            catch (ServiceAuthorizationException ex) // Thrown by EnrollmentService if its internal auth logic fails
            {
                return ForbiddenProblem(ex);
            }
            catch (ServiceBadRequestException ex) // e.g., already enrolled, course not published
            {
                return BadRequestProblem(ex);
            }
        }

        [HttpDelete("unenroll")]
        [Authorize(Roles = "Student,Admin")]
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

            try
            {
                await _enrollmentService.UnenrollAsync(courseId, userIdToUnenroll.Value, cancellationToken);
                return NoContent();
            }
            catch (ServiceNotFoundException ex) // e.g., Course not found, or Enrollment not found
            {
                return NotFoundProblem(ex);
            }
            catch (ServiceAuthorizationException ex) // Thrown by EnrollmentService if its internal auth logic fails
            {
                return ForbiddenProblem(ex);
            }
        }
    }
}