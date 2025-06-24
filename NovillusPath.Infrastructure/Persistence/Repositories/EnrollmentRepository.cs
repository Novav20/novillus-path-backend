using Microsoft.EntityFrameworkCore;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository(NovillusDbContext context) : EfRepository<Enrollment>(context), IEnrollmentRepository
{
    public async Task<Enrollment?> GetByUserIdAndCourseIdAsync(Guid userId, Guid courseId, CancellationToken cancellationToken)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken);
    }
}
