using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SourceGuild.API.Extensions;
using SourceGuild.Application.Constants;
using SourceGuild.Application.DTOs.Common;
using SourceGuild.Application.DTOs.Course;
using SourceGuild.Application.Features.Courses;

namespace SourceGuild.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class CoursesController(CourseCommands courseCommands, CourseQueries courseQueries) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<CourseListProjectionDto>))]
    public async Task<IActionResult> GetCourses([FromQuery] CourseSearchParamsDto searchParams, CancellationToken cancellationToken)
    {
        var courses = await courseQueries.GetPagedListAsync(searchParams, cancellationToken);
        return Ok(courses);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseById(Guid id, CancellationToken cancellationToken)
    {
        var result = await courseQueries.GetByIdAsync(id, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Roles = Roles.Instructor + "," + Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto, CancellationToken cancellationToken)
    {
        var result = await courseCommands.CreateAsync(dto, cancellationToken);
        return result.ToCreatedAtActionResult(nameof(GetCourseById), new { id = result.IsSuccess ? result.Value.Id : Guid.Empty });
    }

    [HttpPatch("{id:guid}/price")]
    [Authorize(Roles = Roles.Instructor + "," + Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] decimal newPrice, CancellationToken cancellationToken)
    {
        var result = await courseCommands.UpdatePriceAsync(id, newPrice, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = Roles.Instructor + "," + Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PublishCourse(Guid id, CancellationToken cancellationToken)
    {
        var result = await courseCommands.PublishAsync(id, cancellationToken);
        return result.ToActionResult();
    }
}