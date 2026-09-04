using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SourceGuild.API.Extensions;
using SourceGuild.Application.Constants;
using SourceGuild.Application.DTOs.Lesson;
using SourceGuild.Application.Features.Courses;

namespace SourceGuild.API.Controllers;

[Route("api/courses/{courseId:guid}/sections/{sectionId:guid}/lessons")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class LessonsController(CourseCommands courseCommands, CourseQueries courseQueries) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<LessonDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLessons(
        [FromRoute] Guid courseId, 
        [FromRoute] Guid sectionId, 
        CancellationToken cancellationToken)
    {
        var courseResult = await courseQueries.GetByIdAsync(courseId, cancellationToken);
        if (courseResult.IsFailure)
            return courseResult.ToActionResult();

        var section = courseResult.Value.Sections?.FirstOrDefault(s => s.Id == sectionId);
        if (section is null)
            return NotFound(new ProblemDetails { Title = "Section.NotFound", Detail = "La sección no pertenece al curso especificado." });

        return Ok(section.Lessons ?? []);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LessonDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateLesson(
        [FromRoute] Guid courseId, 
        [FromRoute] Guid sectionId, 
        [FromBody] CreateLessonDto dto, 
        CancellationToken cancellationToken)
    {
        var result = await courseCommands.AddLessonToSectionAsync(courseId, sectionId, dto, cancellationToken);
        return result.ToActionResult();
    }
}