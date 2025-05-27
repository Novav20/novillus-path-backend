using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;
using NovillusPath.Infrastructure.Persistence;

namespace NovillusPath.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(ICourseRepository courseRepository, IMapper mapper, NovillusDbContext context) : ControllerBase
{
    private readonly ICourseRepository _courseRepository = courseRepository;
    private readonly IMapper _mapper = mapper;
    private readonly NovillusDbContext _context = context;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetCourses(CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.ListAllAsync(cancellationToken);
        var coursesDto = _mapper.Map<IReadOnlyList<CourseDto>>(courses);
        return Ok(coursesDto);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetCourseById(Guid id, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
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
        await _courseRepository.AddAsync(courseToCreate, cancellationToken);

        // TODO: This will be moved to a Unit of Work or Application Service later
        await _context.SaveChangesAsync(cancellationToken);

        var courseDto = _mapper.Map<CourseDto>(courseToCreate);

        return CreatedAtAction(nameof(GetCourseById), new { id = courseDto.Id }, courseDto);
    }
}
