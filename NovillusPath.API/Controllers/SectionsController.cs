using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Section;
using NovillusPath.Application.Interfaces.Services;
using NovillusPath.Application.Constants;
namespace NovillusPath.API.Controllers
{
    [Route("api/courses/{courseId:guid}/sections")]
    [ApiController]
    public class SectionsController(ISectionService sectionService) : BaseApiController
    {
        private readonly ISectionService _sectionService = sectionService; 

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //TODO: Just section 0 (Overview) can be viewed by unenrrolled users
        public async Task<ActionResult<IReadOnlyList<SectionDto>>> GetSections([FromRoute] Guid courseId, CancellationToken cancellationToken)
        {
            var sectionsDto = await _sectionService.GetSectionsAsync(courseId, cancellationToken);
            return Ok(sectionsDto);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDto>> CreateSection([FromRoute] Guid courseId, [FromBody] CreateSectionDto createSectionDto, CancellationToken cancellationToken)
        {
            var sectionDto = await _sectionService.CreateSectionAsync(courseId, createSectionDto, cancellationToken);
            return CreatedAtAction(nameof(GetSectionById), new { courseId, sectionId = sectionDto.Id }, sectionDto);
        }

        [HttpGet("{sectionId:guid}", Name = "GetSectionById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDto>> GetSectionById([FromRoute] Guid courseId, [FromRoute] Guid sectionId, CancellationToken cancellationToken)
        {
            var sectionDto = await _sectionService.GetSectionByIdAsync(courseId, sectionId, cancellationToken);
            return Ok(sectionDto);
        }

        [HttpPut("{sectionId:guid}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSection([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromBody] UpdateSectionDto updateSectionDto, CancellationToken cancellationToken)
        {
            await _sectionService.UpdateSectionAsync(courseId, sectionId, updateSectionDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{sectionId:guid}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteSection([FromRoute] Guid courseId, [FromRoute] Guid sectionId, CancellationToken cancellationToken)
        {
            await _sectionService.DeleteSectionAsync(courseId, sectionId, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{sectionId:guid}/status")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Instructor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSectionStatus([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromBody] UpdateSectionStatusDto updateSectionStatusDto, CancellationToken cancellationToken)
        {
            await _sectionService.UpdateSectionStatusAsync(courseId, sectionId, updateSectionStatusDto, cancellationToken);
            return NoContent();
        }
    }
}
