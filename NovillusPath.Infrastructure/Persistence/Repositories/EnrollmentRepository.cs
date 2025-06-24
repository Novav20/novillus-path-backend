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

    public async Task<IReadOnlyList<Enrollment>> GetEnrollmentsByUserIdAsync(Guid userId, bool includeCourseDetails, CancellationToken cancellationToken)
    {
        IQueryable<Enrollment> query = _context.Enrollments.Where(e => e.UserId == userId);

        if (includeCourseDetails)
        {
            query = query.Include(e => e.Course)
                .ThenInclude(c => c.Categories);
        }
        query = query.OrderByDescending(e => e.EnrolledAt);
        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }
}
