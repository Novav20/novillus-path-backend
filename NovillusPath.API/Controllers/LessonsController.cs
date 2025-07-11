using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Lesson;
using NovillusPath.Application.Exceptions;
using NovillusPath.Application.Interfaces.Services;

namespace NovillusPath.API.Controllers
{
    [Route("api/courses/{courseId:guid}/sections/{sectionId:guid}/lessons")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class LessonsController(ILessonService lessonService) : BaseApiController
    {
        private readonly ILessonService _lessonService = lessonService;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<LessonDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<LessonDto>>> GetLessonsBySection([FromRoute] Guid courseId, [FromRoute] Guid sectionId, CancellationToken cancellationToken)
        {
            try
            {
                var lessons = await _lessonService.GetLessonsBySectionAsync(sectionId, cancellationToken);
                return Ok(lessons);
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFoundProblem(ex);
            }
        }

        [HttpGet("{lessonId:guid}", Name = "GetLessonById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LessonDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LessonDto>> GetLessonById([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromRoute] Guid lessonId, CancellationToken cancellationToken)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(sectionId, lessonId, cancellationToken);
                return Ok(lesson);
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFoundProblem(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LessonDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LessonDto>> CreateLesson([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromBody] CreateLessonDto createLessonDto, CancellationToken cancellationToken)
        {
            try
            {
                var lesson = await _lessonService.CreateLessonAsync(sectionId, createLessonDto, cancellationToken);
                return CreatedAtAction(nameof(GetLessonById), new { courseId, sectionId, lessonId = lesson.Id }, lesson);
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

        [HttpPut("{lessonId:guid}")]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLesson([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromRoute] Guid lessonId, [FromBody] UpdateLessonDto updateLessonDto, CancellationToken cancellationToken)
        {
            try
            {
                await _lessonService.UpdateLessonAsync(sectionId, lessonId, updateLessonDto, cancellationToken);
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

        [HttpPatch("{lessonId:guid}/status")]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLessonStatus([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromRoute] Guid lessonId, [FromBody] UpdateLessonStatusDto updateLessonStatusDto, CancellationToken cancellationToken)
        {
            try
            {
                await _lessonService.UpdateLessonStatusAsync(sectionId, lessonId, updateLessonStatusDto, cancellationToken);
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

        [HttpDelete("{lessonId:guid}")]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteLesson([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromRoute] Guid lessonId, CancellationToken cancellationToken)
        {
            try
            {
                await _lessonService.DeleteLessonAsync(sectionId, lessonId, cancellationToken);
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


    }
}
