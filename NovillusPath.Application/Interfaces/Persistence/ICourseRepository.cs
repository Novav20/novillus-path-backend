using System.Linq.Expressions;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Interfaces.Persistence;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetDetailedCourseConditionallyAsync(Guid id, bool includeSections = false, bool includeLessonsInSections = false, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<Course?> GetFullCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CourseListProjectionDto>> GetFilteredCoursesAsync(Expression<Func<Course, bool>>? filterPredicate,CancellationToken cancellationToken);
}
