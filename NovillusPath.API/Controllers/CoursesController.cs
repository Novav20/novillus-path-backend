using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetCourses(CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.CourseRepository.ListAllAsync(cancellationToken);
        var coursesDto = _mapper.Map<IReadOnlyList<CourseDto>>(courses);
        return Ok(coursesDto);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetCourseById(Guid id, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id, cancellationToken);
        if (course == null) return NotFound($"Course with ID {id} not found.");
        var courseDto = _mapper.Map<CourseDto>(course);
        return Ok(courseDto);

    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto, CancellationToken cancellationToken)
    {
        var instructorId = _currentUserService.UserId;
        if (!instructorId.HasValue) return Unauthorized("Instructor ID could not be determined from token.");

        var courseToCreate = _mapper.Map<Course>(createCourseDto);
        courseToCreate.InstructorId = instructorId.Value;

        if (createCourseDto.CategoryIds != null && createCourseDto.CategoryIds.Count > 0)
        {
            var categories = await _unitOfWork.CategoryRepository
                .ListAsync(c => createCourseDto.CategoryIds.Contains(c.Id), cancellationToken);

            if (categories.Count != createCourseDto.CategoryIds.Distinct().Count())
            {
                var foundIds = categories.Select(c => c.Id).ToList();
                var missingIds = createCourseDto.CategoryIds.Except(foundIds).ToList();
                return BadRequest($"Categories with IDs {string.Join(", ", missingIds)} not found.");
            }

            courseToCreate.Categories = [.. categories];

        }

        await _unitOfWork.CourseRepository.AddAsync(courseToCreate, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var courseDto = _mapper.Map<CourseDto>(courseToCreate); // TODO: We'll update this DTO mapping later

        return CreatedAtAction(nameof(GetCourseById), new { id = courseDto.Id }, courseDto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]

    public async Task<ActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id, cancellationToken);
        if (course == null) return NotFound($"Course with ID {id} not found.");

        var currentUserId = _currentUserService.UserId;
        bool isAdmin = _currentUserService.IsInRole("Admin");

        if (!isAdmin && course.InstructorId != currentUserId)
        {
            return Forbid("You are not authorized to update this course.");
        }

        _mapper.Map(updateCourseDto, course);

        if (updateCourseDto.CategoryIds != null)
        {
            if (updateCourseDto.CategoryIds.Count == 0)
            {
                course.Categories.Clear();
            }
            else
            {
                var categoriesFromDto = await _unitOfWork.CategoryRepository
                    .ListAsync(c => updateCourseDto.CategoryIds.Contains(c.Id), cancellationToken);

                if (categoriesFromDto.Count != updateCourseDto.CategoryIds.Distinct().Count())
                {
                    var foundIds = categoriesFromDto.Select(c => c.Id).ToList();
                    var missingIds = updateCourseDto.CategoryIds.Except(foundIds).ToList();
                    return BadRequest($"Categories with IDs {string.Join(", ", missingIds)} not found.");
                }
                course.Categories.Clear();
                course.Categories = [.. categoriesFromDto];
            }
        }
        course.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteCourse(Guid id, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id, cancellationToken);
        if (course == null) return NotFound($"Course with ID {id} not found.");

        var currentUserId = _currentUserService.UserId;
        bool isAdmin = _currentUserService.IsInRole("Admin");

        if (!isAdmin && course.InstructorId != currentUserId)
        {
            return Forbid("You are not authorized to delete this course.");
        }
        await _unitOfWork.CourseRepository.DeleteAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
