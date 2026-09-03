using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository(SGDbContext context) : EfRepository<Enrollment>(context), IEnrollmentRepository
{
    public async Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByUserIdAsync(Guid userId, bool includeCourseDetails = true, CancellationToken cancellationToken = default)
    {
        IQueryable<Enrollment> query = _context.Enrollments.Where(e => e.UserId == userId);

        if (includeCourseDetails)
        {
            query = query.Include(e => e.Course)
                .ThenInclude(c => c.Categories);
        }

        return await query.OrderByDescending(e => e.EnrolledAt)
                          .AsNoTracking()
                          .ToListAsync(cancellationToken);
    }
}