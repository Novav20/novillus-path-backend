using SourceGuild.Application.Common.Mappings;
using SourceGuild.Application.DTOs.Common;
using SourceGuild.Application.DTOs.Course;
using SourceGuild.Application.DTOs.Dashboard;
using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Common;

namespace SourceGuild.Application.Features.Courses;

public class CourseQueries(ICourseRepository courseRepository)
{
    public async Task<Result<CourseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetWithDetailsAsync(id, cancellationToken);
        if (course is null)
            return Result<CourseDto>.Failure(Error.NotFound("Course.NotFound", "El curso solicitado no existe."));

        return Result<CourseDto>.Success(course.ToDto());
    }

    public async Task<PagedResult<CourseListProjectionDto>> GetPagedListAsync(
        CourseSearchParamsDto searchParams,
        CancellationToken cancellationToken = default)
    {
        return await courseRepository.GetFilteredCoursesAsync(searchParams, null, cancellationToken);
    }

    public async Task<IReadOnlyList<CreatedCourseSummaryDto>> GetInstructorCoursesAsync(
        Guid instructorId,
        CancellationToken cancellationToken = default)
    {
        return await courseRepository.GetInstructorCoursesSummaryAsync(instructorId, cancellationToken);
    }
}