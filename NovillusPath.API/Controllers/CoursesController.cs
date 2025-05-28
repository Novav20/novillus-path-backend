using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto, CancellationToken cancellationToken)
    {
        // TODO: In Week 2, when we have auth, we'll get InstructorId from the logged-in user.
        // For now, createCourseDto.InstructorId is used. We might also validate if this InstructorId exists.

        var courseToCreate = _mapper.Map<Course>(createCourseDto);
        await _unitOfWork.CourseRepository.AddAsync(courseToCreate, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var courseDto = _mapper.Map<CourseDto>(courseToCreate);

        return CreatedAtAction(nameof(GetCourseById), new { id = courseDto.Id }, courseDto);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id, cancellationToken);
        if (course == null) return NotFound($"Course with ID {id} not found.");
        // TODO: Authorization check (Week 2) - e.g., is current user the instructor?
        _mapper.Map(updateCourseDto, course);
        course.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCourse(Guid id, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id, cancellationToken);
        if (course == null) return NotFound($"Course with ID {id} not found.");
        // TODO: Authorization check (Week 2) - e.g., just owner or admin can delete course
        await _unitOfWork.CourseRepository.DeleteAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
