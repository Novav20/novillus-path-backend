using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Infrastructure.Persistence.Repositories;

public class ReviewRepository(SGDbContext context) : EfRepository<Review>(context), IReviewRepository
{
    public async Task<IReadOnlyList<Review>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => r.CourseId == courseId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Review> Reviews, int TotalCount)> GetPagedReviewsByCourseIdAsync(
        Guid courseId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Reviews
            .Where(r => r.CourseId == courseId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var reviews = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (reviews, totalCount);
    }
}