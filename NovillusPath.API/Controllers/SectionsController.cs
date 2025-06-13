using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Section;
using NovillusPath.Application.Interfaces.Services;
using NovillusPath.Application.Services;
namespace NovillusPath.API.Controllers
{
    [Route("api/courses/{courseId:guid}/sections")]
    [ApiController]
    public class SectionsController(ISectionService sectionService) : ControllerBase
    {
        private readonly ISectionService _sectionService = sectionService; 

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //TODO: Just section 0 (Overview) can be viewed by unenrrolled users
        public async Task<ActionResult<IReadOnlyList<SectionDto>>> GetSections([FromRoute] Guid courseId, CancellationToken cancellationToken)
        {
            try
            {
                var sectionsDto = await _sectionService.GetSectionsAsync(courseId, cancellationToken);
                return Ok(sectionsDto);
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDto>> CreateSection([FromRoute] Guid courseId, [FromBody] CreateSectionDto createSectionDto, CancellationToken cancellationToken)
        {
            try
            {
                var sectionDto = await _sectionService.CreateSectionAsync(courseId, createSectionDto, cancellationToken);
                return CreatedAtAction(nameof(GetSectionById), new { courseId, sectionId = sectionDto.Id }, sectionDto);
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ServiceAuthorizationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ServiceBadRequestException ex) 
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("{sectionId:guid}", Name = "GetSectionById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDto>> GetSectionById([FromRoute] Guid courseId, [FromRoute] Guid sectionId, CancellationToken cancellationToken)
        {
            try
            {
                var sectionDto = await _sectionService.GetSectionByIdAsync(courseId, sectionId, cancellationToken);
                return Ok(sectionDto);
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{sectionId:guid}")]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSection([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromBody] UpdateSectionDto updateSectionDto, CancellationToken cancellationToken)
        {
            try
            {
                await _sectionService.UpdateSectionAsync(courseId, sectionId, updateSectionDto, cancellationToken);
                return NoContent();
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ServiceAuthorizationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ServiceBadRequestException ex) 
            {
                return BadRequest(ex.Message); 
            }
            // ASP.NET Core handles model validation (BadRequest) automatically.
        }

        [HttpDelete("{sectionId:guid}")]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteSection([FromRoute] Guid courseId, [FromRoute] Guid sectionId, CancellationToken cancellationToken)
        {
            try
            {
                await _sectionService.DeleteSectionAsync(courseId, sectionId, cancellationToken);
                return NoContent();
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ServiceAuthorizationException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
