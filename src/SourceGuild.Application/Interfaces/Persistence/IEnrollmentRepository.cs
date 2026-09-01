namespace SourceGuild.Application.Interfaces.Persistence;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetByUserIdAndCourseIdAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Enrollment>> GetEnrollmentsByUserIdAsync(Guid userId, bool includeCourseDetails, CancellationToken cancellationToken);
}
