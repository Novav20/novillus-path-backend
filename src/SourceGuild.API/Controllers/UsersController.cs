using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SourceGuild.Application.Constants;
using SourceGuild.Application.DTOs.Dashboard;
using SourceGuild.Application.Features.Enrollments;

namespace SourceGuild.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(EnrollmentFeatures enrollmentFeatures) : ControllerBase
{
    [HttpGet("me/my-learning")]
    [Authorize(Roles = Roles.Student)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<EnrolledCourseSummaryDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyEnrolledCourses(CancellationToken cancellationToken)
    {
        var courses = await enrollmentFeatures.GetStudentEnrollmentsAsync(cancellationToken);
        return Ok(courses);
    }
}