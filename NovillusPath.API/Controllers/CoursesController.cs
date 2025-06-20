using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Exceptions;
using NovillusPath.Application.Interfaces.Services;
using System.Net.Mime;

namespace NovillusPath.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class CoursesController(ICourseService courseService) : BaseApiController
{
    private readonly ICourseService _courseService = courseService;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<CourseDto>))]
    public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetCourses(CancellationToken cancellationToken)
    {
        var courses = await _courseService.GetCoursesAsync(cancellationToken);
        return Ok(courses);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetCourseById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseService.GetCourseByIdAsync(id, cancellationToken);
            return Ok(course);
        }
        catch (ServiceNotFoundException ex)
        {
            return NotFoundProblem(ex);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseService.CreateCourseAsync(createCourseDto, cancellationToken);
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }
        catch (ServiceAuthorizationException ex)
        {
            return UnauthorizedProblem(ex);
        }
        catch (ServiceBadRequestException ex)
        {
            return BadRequestProblem(ex);
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto, CancellationToken cancellationToken)
    {
        try
        {
            await _courseService.UpdateCourseAsync(id, updateCourseDto, cancellationToken);
            return NoContent();
        }
        catch (ServiceNotFoundException ex)
        {
            return NotFoundProblem(ex);
        }
        catch (ServiceAuthorizationException ex)
        {
            return ForbiddenProblem(ex);
        }
        catch (ServiceBadRequestException ex)
        {
            return BadRequestProblem(ex);
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCourse(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _courseService.DeleteCourseAsync(id, cancellationToken);
            return NoContent();
        }
        catch (ServiceNotFoundException ex)
        {
            return NotFoundProblem(ex);
        }
        catch (ServiceAuthorizationException ex)
        {
            return ForbiddenProblem(ex);
        }
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCourseStatus(Guid id, [FromBody] UpdateCourseStatusDto updateCourseStatusDto, CancellationToken cancellationToken)
    {
        try
        {
            await _courseService.UpdateCourseStatusAsync(id, updateCourseStatusDto, cancellationToken);
            return NoContent();
        }
        catch (ServiceNotFoundException ex)
        {
            return NotFoundProblem(ex);
        }
        catch (ServiceAuthorizationException ex)
        {
            return ForbiddenProblem(ex);
        }
        catch (ServiceBadRequestException ex)
        {
            return BadRequestProblem(ex);
        }
    }
}
