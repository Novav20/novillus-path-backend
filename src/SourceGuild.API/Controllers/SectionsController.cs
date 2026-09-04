using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SourceGuild.API.Extensions;
using SourceGuild.Application.Constants;
using SourceGuild.Application.DTOs.Section;
using SourceGuild.Application.Features.Courses;

namespace SourceGuild.API.Controllers;

[Route("api/courses/{courseId:guid}/sections")]
[ApiController]
public class SectionsController(CourseCommands courseCommands, CourseQueries courseQueries) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<SectionDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSections([FromRoute] Guid courseId, CancellationToken cancellationToken)
    {
        var courseResult = await courseQueries.GetByIdAsync(courseId, cancellationToken);
        if (courseResult.IsFailure)
            return courseResult.ToActionResult();

        return Ok(courseResult.Value.Sections ?? []);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SectionDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSection(
        [FromRoute] Guid courseId, 
        [FromBody] CreateSectionDto dto, 
        CancellationToken cancellationToken)
    {
        var result = await courseCommands.AddSectionAsync(courseId, dto, cancellationToken);
        return result.ToActionResult();
    }
}