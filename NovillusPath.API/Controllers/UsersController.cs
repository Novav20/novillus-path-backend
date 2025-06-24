using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Services;

namespace NovillusPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IEnrollmentService enrollmentService, ICurrentUserService currentUserService) : BaseApiController
    {
        private readonly IEnrollmentService _enrollmentService = enrollmentService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        [HttpGet("me/my-learning")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<CourseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // If not authenticated
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetMyEnrolledCourses(CancellationToken cancellationToken = default)
        {

            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "User ID could not be determined.", Status = StatusCodes.Status401Unauthorized });
            }
            var enrolledCourses = await _enrollmentService.GetUserEnrolledCoursesAsync(userId.Value, cancellationToken);
            return Ok(enrolledCourses);
        }

    }
}
