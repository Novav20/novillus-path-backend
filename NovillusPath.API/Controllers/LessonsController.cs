namespace NovillusPath.API.Controllers
{
    /// <summary>
    /// API controller for managing lessons within sections of a course.
    /// </summary>
    [Route("api/courses/{courseId:guid}/sections/{sectionId:guid}/lessons")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class LessonsController(ILessonService lessonService) : ControllerBase
    {
        private readonly ILessonService _lessonService = lessonService;

        /// <summary>
        /// Retrieves a list of lessons for a specific section.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of LessonDto.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<LessonDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<LessonDto>>> GetLessonsBySection([FromRoute] Guid courseId, [FromRoute] Guid sectionId, CancellationToken cancellationToken)
        {
            var lessons = await _lessonService.GetLessonsBySectionAsync(sectionId, cancellationToken);
            return Ok(lessons);
        }

        /// <summary>
        /// Retrieves a specific lesson by its ID within a section.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="lessonId">The ID of the lesson.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The LessonDto.</returns>
        [HttpGet("{lessonId:guid}", Name = "GetLessonById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LessonDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LessonDto>> GetLessonById([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromRoute] Guid lessonId, CancellationToken cancellationToken)
        {
            var lesson = await _lessonService.GetLessonByIdAsync(sectionId, lessonId, cancellationToken);
            return Ok(lesson);
        }

        /// <summary>
        /// Creates a new lesson within a specific section.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="createLessonDto">The lesson data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created LessonDto.</returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LessonDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LessonDto>> CreateLesson([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromBody] CreateLessonDto createLessonDto, CancellationToken cancellationToken)
        {
            var lesson = await _lessonService.CreateLessonAsync(sectionId, createLessonDto, cancellationToken);
            return CreatedAtAction(nameof(GetLessonById), new { courseId, sectionId, lessonId = lesson.Id }, lesson);
        }

        /// <summary>
        /// Updates an existing lesson within a specific section.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="lessonId">The ID of the lesson to update.</param>
        /// <param name="updateLessonDto">The updated lesson data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPut("{lessonId:guid}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLesson([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromRoute] Guid lessonId, [FromBody] UpdateLessonDto updateLessonDto, CancellationToken cancellationToken)
        {
            await _lessonService.UpdateLessonAsync(sectionId, lessonId, updateLessonDto, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates the status of a lesson.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="lessonId">The ID of the lesson.</param>
        /// <param name="updateLessonStatusDto">The new status.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPatch("{lessonId:guid}/status")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLessonStatus([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromRoute] Guid lessonId, [FromBody] UpdateLessonStatusDto updateLessonStatusDto, CancellationToken cancellationToken)
        {
            await _lessonService.UpdateLessonStatusAsync(sectionId, lessonId, updateLessonStatusDto, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Deletes a lesson by its ID within a section.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="lessonId">The ID of the lesson to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{lessonId:guid}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteLesson([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromRoute] Guid lessonId, CancellationToken cancellationToken)
        {
            await _lessonService.DeleteLessonAsync(sectionId, lessonId, cancellationToken);
            return NoContent();
        }


    }
}
