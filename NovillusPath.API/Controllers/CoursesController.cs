namespace NovillusPath.API.Controllers;

/// <summary>
/// API controller for managing courses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class CoursesController(ICourseService courseService) : BaseApiController
{
    private readonly ICourseService _courseService = courseService;

    /// <summary>
    /// Retrieves a paginated list of courses based on search parameters.
    /// </summary>
    /// <param name="searchParams">Search, filter, and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of CourseDto.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<CourseDto>))]
    public async Task<ActionResult<PagedResult<CourseDto>>> GetCourses([FromQuery] CourseSearchParamsDto searchParams, CancellationToken cancellationToken)
    {
        var courses = await _courseService.GetCoursesAsync(searchParams, cancellationToken);
        return Ok(courses);
    }

    /// <summary>
    /// Retrieves a course by its ID.
    /// </summary>
    /// <param name="id">The ID of the course.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The CourseDto.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetCourseById(Guid id, CancellationToken cancellationToken)
    {
        var course = await _courseService.GetCourseByIdAsync(id, cancellationToken);
        return Ok(course);
    }

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="createCourseDto">The course data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created CourseDto.</returns>
    [HttpPost]
    [Authorize(Roles = Roles.Instructor + "," + Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto, CancellationToken cancellationToken)
    {
        var course = await _courseService.CreateCourseAsync(createCourseDto, cancellationToken);
        return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
    }

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    /// <param name="id">The ID of the course to update.</param>
    /// <param name="updateCourseDto">The updated course data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Instructor + "," + Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto, CancellationToken cancellationToken)
    {
        await _courseService.UpdateCourseAsync(id, updateCourseDto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes a course by its ID.
    /// </summary>
    /// <param name="id">The ID of the course to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Instructor + "," + Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCourse(Guid id, CancellationToken cancellationToken)
    {
        await _courseService.DeleteCourseAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Updates the status of a course.
    /// </summary>
    /// <param name="id">The ID of the course.</param>
    /// <param name="updateCourseStatusDto">The new status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Instructor + "," + Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCourseStatus(Guid id, [FromBody] UpdateCourseStatusDto updateCourseStatusDto, CancellationToken cancellationToken)
    {
        await _courseService.UpdateCourseStatusAsync(id, updateCourseStatusDto, cancellationToken);
        return NoContent();
    }
}
