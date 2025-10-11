namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class ReviewRepository(NovillusDbContext context) : EfRepository<Review>(context), IReviewRepository
{
    public async Task<(IReadOnlyList<Review> Reviews, int TotalCount)> GetPagedReviewsByCourseIdWithUserDetailsAsync(Guid courseId, int pageNumber, int pageSize, CancellationToken cancellationToken)
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

    public async Task<Review?> GetReviewDetailsByIdAsync(Guid reviewId, CancellationToken token)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == reviewId, token);
    }

    public async Task<IReadOnlyList<Review>> GetReviewsByCourseIdWithUserDetailsAsync(Guid courseId, CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Where(r => r.CourseId == courseId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }


}
