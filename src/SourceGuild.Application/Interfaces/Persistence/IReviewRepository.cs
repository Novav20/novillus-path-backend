using SourceGuild.Domain.Entities;

namespace SourceGuild.Application.Interfaces.Persistence;

public interface IReviewRepository : IRepository<Review>
{
    Task<IReadOnlyList<Review>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Review> Reviews, int TotalCount)> GetPagedReviewsByCourseIdAsync(
        Guid courseId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}