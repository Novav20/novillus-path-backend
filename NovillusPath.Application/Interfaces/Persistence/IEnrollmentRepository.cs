using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Interfaces.Persistence;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetByUserIdAndCourseIdAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
}
