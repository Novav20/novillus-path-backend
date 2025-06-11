using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Section;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.API.Controllers
{
    [Route("api/courses/{courseId:guid}/sections")]
    [ApiController]
    public class SectionsController(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //TODO: Just section 0 (Overview) can be viewed by unenrrolled users
        public async Task<ActionResult<IReadOnlyList<SectionDto>>> GetSections([FromRoute] Guid courseId, CancellationToken cancellationToken)
        {
            // Use ExistsAsync for efficient existence check
            bool courseExists = await _unitOfWork.CourseRepository.ExistsAsync(c => c.Id == courseId, cancellationToken);
            if (!courseExists) return NotFound($"Course with ID {courseId} not found.");

            var sections = await _unitOfWork.SectionRepository.ListAsync(s => s.CourseId == courseId, s => s.Order, true, cancellationToken);

            var sectionsDto = _mapper.Map<IReadOnlyList<SectionDto>>(sections);
            return Ok(sectionsDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDto>> CreateSection([FromRoute] Guid courseId, [FromBody] CreateSectionDto createSectionDto, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.CourseRepository.GetCourseWithDetailsAsync(courseId, cancellationToken);
            if (course == null) return NotFound($"Course with ID {courseId} not found.");

            if (course.InstructorId != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
            {
                return Forbid("You are not authorized to create sections in this course.");
            }

            var sectionToCreate = _mapper.Map<Section>(createSectionDto);
            sectionToCreate.CourseId = courseId;

            if (createSectionDto.Order == null) // If client doesn't specify order, append it
            {
                sectionToCreate.Order = course.Sections.Count != 0 ? course.Sections.Max(s => s.Order) + 1 : 0;
            }
            else // If client specifies order, use it (potential for conflicts if not handled, or re-ordering logic needed later)
            {
                sectionToCreate.Order = createSectionDto.Order.Value;
                // TODO: Handle potential duplicate Order values or implement re-ordering if an existing order is chosen.
            }

            await _unitOfWork.SectionRepository.AddAsync(sectionToCreate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var sectionDto = _mapper.Map<SectionDto>(sectionToCreate);
            return CreatedAtAction(nameof(GetSectionById), new { courseId, sectionId = sectionToCreate.Id }, sectionDto);
        }

        [HttpGet("{sectionId:guid}", Name = "GetSectionById")] // Added Name for CreatedAtAction
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDto>> GetSectionById([FromRoute] Guid courseId, [FromRoute] Guid sectionId, CancellationToken cancellationToken)
        {
            var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId, cancellationToken);

            if (section == null || section.CourseId != courseId) // Crucial: ensure section belongs to this course
            {
                return NotFound($"Section with ID {sectionId} not found in course {courseId}.");
            }

            var sectionDto = _mapper.Map<SectionDto>(section);
            return Ok(sectionDto);
        }

        [HttpPut("{sectionId:guid}")]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSection([FromRoute] Guid courseId, [FromRoute] Guid sectionId, [FromBody] UpdateSectionDto updateSectionDto, CancellationToken cancellationToken)
        {
            // 1. Fetch the course for authorization check
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken); // Does not need sections for this check
            if (course == null)
            {
                return NotFound($"Course with ID {courseId} not found.");
            }

            // 2. Authorization: Must be Admin or the Instructor who owns the course
            if (course.InstructorId != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
            {
                return Forbid("You are not authorized to update sections in this course.");
            }

            // 3. Fetch the actual section to update
            var sectionToUpdate = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId, cancellationToken);
            if (sectionToUpdate == null || sectionToUpdate.CourseId != courseId)
            {
                return NotFound($"Section with ID {sectionId} not found in course {courseId} for update.");
            }

            // 4. Map DTO to entity (AutoMapper handles partials due to your profile config)
            _mapper.Map(updateSectionDto, sectionToUpdate);
            sectionToUpdate.UpdatedAt = DateTime.UtcNow;
            // TODO: Add logic here if Order is changed to shift other sections' orders.
            // For now, it directly updates the Order. If this creates duplicates, that's a current limitation.

            // _unitOfWork.SectionRepository.UpdateAsync(sectionToUpdate); // Not strictly needed if entity is tracked
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{sectionId:guid}")]
        [Authorize(Roles = "Admin,Instructor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteSection([FromRoute] Guid courseId, [FromRoute] Guid sectionId, CancellationToken cancellationToken)
        {
            // 1. Fetch the course for authorization check
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken);
            if (course == null)
            {
                return NotFound($"Course with ID {courseId} not found.");
            }

            // 2. Authorization: Must be Admin or the Instructor who owns the course
            if (course.InstructorId != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
            {
                return Forbid("You are not authorized to delete sections in this course.");
            }

            // 3. Fetch the actual section to delete
            var sectionToDelete = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId, cancellationToken);
            if (sectionToDelete == null || sectionToDelete.CourseId != courseId)
            {
                return NotFound($"Section with ID {sectionId} not found in course {courseId} for deletion.");
            }

            // TODO: If lessons are part of a section and have a Cascade delete behavior with Section, they'll be deleted.
            // If not, you might need to handle deleting lessons first or prevent deleting a section with lessons.
            // For now, our Section->Lesson relationship isn't defined yet.

            await _unitOfWork.SectionRepository.DeleteAsync(sectionToDelete, cancellationToken);
            // TODO: Add logic here to re-order subsequent sections if desired.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
