using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Interfaces.Persistence;

public interface IReviewRepository : IRepository<Review>
{
    Task<Review?> GetReviewDetailsByIdAsync(Guid reviewId, CancellationToken token);
    Task<IReadOnlyList<Review>> GetReviewsByCourseIdWithUserDetailsAsync(Guid courseId, CancellationToken cancellationToken);
}
