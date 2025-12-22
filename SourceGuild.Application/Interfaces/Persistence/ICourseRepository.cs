using System.Linq.Expressions;
using SourceGuild.Application.DTOs.Course;
using SourceGuild.Application.DTOs.Dashboard;

namespace SourceGuild.Application.Interfaces.Persistence;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetDetailedCourseConditionallyAsync(Guid id, bool includeSections = false, bool includeLessonsInSections = false, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<Course?> GetFullCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
    Task<PagedResult<CourseListProjectionDto>> GetFilteredCoursesAsync(CourseSearchParamsDto searchParams, Expression<Func<Course, bool>>? filterPredicate, CancellationToken cancellationToken);
    Task<IReadOnlyList<CreatedCourseSummaryDto>> GetInstructorCoursesSummaryAsync(Guid instructorId, CancellationToken cancellationToken);
}
