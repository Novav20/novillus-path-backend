using System.Linq.Expressions;
using SourceGuild.Application.DTOs.Common;
using SourceGuild.Application.DTOs.Course;
using SourceGuild.Application.DTOs.Dashboard;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Application.Interfaces.Persistence;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<CourseListProjectionDto>> GetFilteredCoursesAsync(CourseSearchParamsDto searchParams, Expression<Func<Course, bool>>? filterPredicate = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CreatedCourseSummaryDto>> GetInstructorCoursesSummaryAsync(Guid instructorId, CancellationToken cancellationToken = default);
}