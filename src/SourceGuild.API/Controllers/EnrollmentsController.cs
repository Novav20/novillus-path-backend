using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SourceGuild.API.Extensions;
using SourceGuild.Application.Constants;
using SourceGuild.Application.Features.Enrollments;

namespace SourceGuild.API.Controllers;

[Route("api/courses/{courseId:guid}")]
[ApiController]
[Produces("application/json")]
public class EnrollmentsController(EnrollmentFeatures enrollmentFeatures) : ControllerBase
{
    [HttpPost("enroll")]
    [Authorize(Roles = Roles.Student + "," + Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enroll([FromRoute] Guid courseId, CancellationToken cancellationToken)
    {
        var result = await enrollmentFeatures.EnrollAsync(courseId, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("progress")]
    [Authorize(Roles = Roles.Student)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProgress([FromRoute] Guid courseId, [FromBody] int progressPercentage, CancellationToken cancellationToken)
    {
        var result = await enrollmentFeatures.UpdateProgressAsync(courseId, progressPercentage, cancellationToken);
        return result.ToActionResult();
    }
}