using NovillusPath.Application.DTOs.Course;

namespace NovillusPath.Application.Interfaces.Services
{
    public interface ICourseService
    {
        Task<PagedResult<CourseDto>> GetCoursesAsync(CourseSearchParamsDto searchParams, CancellationToken cancellationToken);
        Task<CourseDto> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto, CancellationToken cancellationToken);
        Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto, CancellationToken cancellationToken);
        Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken);
        Task UpdateCourseStatusAsync(Guid id, UpdateCourseStatusDto updateStatusDto, CancellationToken cancellationToken);
    }
}
