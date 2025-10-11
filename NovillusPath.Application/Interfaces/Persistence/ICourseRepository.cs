using System.Linq.Expressions;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.DTOs.Dashboard;

namespace NovillusPath.Application.Interfaces.Persistence;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetDetailedCourseConditionallyAsync(Guid id, bool includeSections = false, bool includeLessonsInSections = false, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<Course?> GetFullCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
    Task<PagedResult<CourseListProjectionDto>> GetFilteredCoursesAsync(CourseSearchParamsDto searchParams, Expression<Func<Course, bool>>? filterPredicate, CancellationToken cancellationToken);
    Task<IReadOnlyList<CreatedCourseSummaryDto>> GetInstructorCoursesSummaryAsync(Guid instructorId, CancellationToken cancellationToken);
}
