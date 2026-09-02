using SourceGuild.Domain.Entities;

namespace SourceGuild.Application.Interfaces.Persistence;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Enrollment>> GetByUserIdAsync(Guid userId, bool includeCourseDetails = true, CancellationToken cancellationToken = default);
}