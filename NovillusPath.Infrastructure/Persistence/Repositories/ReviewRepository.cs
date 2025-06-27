using Microsoft.EntityFrameworkCore;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class ReviewRepository(NovillusDbContext context) : EfRepository<Review>(context), IReviewRepository
{
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
