namespace NovillusPath.Application.Interfaces.Persistence;

public interface IReviewRepository : IRepository<Review>
{
    Task<Review?> GetReviewDetailsByIdAsync(Guid reviewId, CancellationToken token);
    Task<IReadOnlyList<Review>> GetReviewsByCourseIdWithUserDetailsAsync(Guid courseId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets a paginated list of reviews for a course, including user details.
    /// </summary>
    /// <param name="courseId">The course ID.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple with the paginated reviews and the total count.</returns>
    Task<(IReadOnlyList<Review> Reviews, int TotalCount)> GetPagedReviewsByCourseIdWithUserDetailsAsync(
        Guid courseId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}
