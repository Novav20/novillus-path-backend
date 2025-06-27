using System.Linq.Expressions;
using AutoMapper;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Exceptions;
using NovillusPath.Application.Helpers;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Application.Interfaces.Services;
using NovillusPath.Domain.Entities;
using NovillusPath.Domain.Enums;

namespace NovillusPath.Application.Services;

public class CourseService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : ICourseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<IReadOnlyList<CourseDto>> GetCoursesAsync(CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        bool isAdmin = _currentUserService.IsInRole("Admin");
        bool isInstructor = _currentUserService.IsInRole("Instructor");
        Expression<Func<Course, bool>>? filter = null;
        if (!isAdmin)
        {
            filter = isInstructor && currentUserId.HasValue
                ? (c => (c.InstructorId == currentUserId.Value) || (c.Status == CourseStatus.Published))
                : (c => c.Status == CourseStatus.Published);
        }
        var courses = await _unitOfWork.CourseRepository.GetFilteredCoursesAsync(filter, cancellationToken);
        return _mapper.Map<IReadOnlyList<CourseDto>>(courses);
    }

    public async Task<CourseDto> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetFullCourseByIdAsync(id, cancellationToken)
            ?? throw new ServiceNotFoundException($"Course with ID {id} not found.");
        var currentUserId = _currentUserService.UserId;
        bool isAdmin = _currentUserService.IsInRole("Admin");
        bool isInstructor = _currentUserService.IsInRole("Instructor");
        if (!VisibilityHelper.CanUserViewCourse(course, _currentUserService))
        {
            throw new ServiceNotFoundException($"Course with ID {id} not found.");
        }
        bool isPublicOrStudentView = !isAdmin && !(isInstructor && course.InstructorId == currentUserId);
        if (isPublicOrStudentView)
        {
            course.Sections = [.. course.Sections.Where(s => s.Status == SectionStatus.Published)];
            foreach (var section in course.Sections)
            {
                section.Lessons = [.. section.Lessons.Where(l => l.Status == LessonStatus.Published)];
            }
        }
        return _mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto, CancellationToken cancellationToken)
    {
        var instructorId = _currentUserService.UserId;
        if (!instructorId.HasValue)
            throw new ServiceAuthorizationException("Instructor ID could not be determined from token.");
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
                throw new ServiceBadRequestException($"Categories with IDs {string.Join(", ", missingIds)} not found.");
            }
            courseToCreate.Categories = [.. categories];
        }
        await _unitOfWork.CourseRepository.AddAsync(courseToCreate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CourseDto>(courseToCreate);
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {id} not found.");
        if (!AuthorizationHelper.CanEditCourse(_currentUserService, course.InstructorId))
            throw new ServiceAuthorizationException("You are not authorized to update this course.");
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
                    throw new ServiceBadRequestException($"Categories with IDs {string.Join(", ", missingIds)} not found.");
                }
                course.Categories.Clear();
                course.Categories = [.. categoriesFromDto];
            }
        }
        course.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(id, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {id} not found.");
        if (!AuthorizationHelper.CanEditCourse(_currentUserService, course.InstructorId))
            throw new ServiceAuthorizationException("You are not authorized to delete this course.");
        await _unitOfWork.CourseRepository.DeleteAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCourseStatusAsync(Guid id, UpdateCourseStatusDto updateStatusDto, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<CourseStatus>(updateStatusDto.NewStatus, true, out var newStatusEnum))
        {
            throw new ServiceBadRequestException($"Invalid status: {updateStatusDto.NewStatus}. Valid values are {string.Join(", ", Enum.GetNames<CourseStatus>())}.");
        }
        var course = await _unitOfWork.CourseRepository.GetDetailedCourseConditionallyAsync(id, true, true, false, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {id} not found.");
        if (!AuthorizationHelper.CanEditCourse(_currentUserService, course.InstructorId))
        {
            throw new ServiceAuthorizationException("You are not authorized to update the status of this course.");
        }
        if (course.Status != newStatusEnum && (newStatusEnum == CourseStatus.Draft || newStatusEnum == CourseStatus.Archived))
        {
            SectionStatus targetSectionStatus = (newStatusEnum == CourseStatus.Draft) ? SectionStatus.Draft : SectionStatus.Archived;
            LessonStatus targetLessonStatus = (newStatusEnum == CourseStatus.Draft) ? LessonStatus.Draft : LessonStatus.Archived;
            if (course.Sections.Count > 0)
            {
                foreach (var section in course.Sections)
                {
                    if (section.Status != targetSectionStatus)
                    {
                        section.Status = targetSectionStatus;
                        section.UpdatedAt = DateTime.UtcNow;
                    }
                    if (section.Lessons.Count > 0)
                    {
                        foreach (var lesson in section.Lessons)
                        {
                            if (lesson.Status != targetLessonStatus)
                            {
                                lesson.Status = targetLessonStatus;
                                lesson.UpdatedAt = DateTime.UtcNow;
                            }
                        }
                    }
                }
            }
        }
        course.Status = newStatusEnum;
        course.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

