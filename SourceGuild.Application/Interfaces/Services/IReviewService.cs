using SourceGuild.Application.DTOs.Review;

namespace SourceGuild.Application.Interfaces.Services;

/// <summary>
/// Service for managing course reviews.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Creates a new review for a specified course by a specified user.
    /// </summary>
    /// <param name="courseId">The ID of the course being reviewed.</param>
    /// <param name="createReviewDto">The review data (Rating, Comment).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created ReviewDto.</returns>
    /// <exception cref="ServiceNotFoundException">If the course is not found.</exception>
    /// <exception cref="ServiceBadRequestException">If validation fails (e.g., user not enrolled, already reviewed, invalid rating).</exception>
    /// <exception cref="ServiceAuthorizationException">If the user is not authorized to submit a review (e.g. not a student, or trying to review for someone else if not admin).</exception>
    Task<ReviewDto> CreateReviewAsync(Guid courseId, CreateReviewDto createReviewDto, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a review by its ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to retrieve.</param>
    /// <param name="courseId">The ID of the course being reviewed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ReviewDto representing the retrieved review.</returns>
    /// <exception cref="ServiceNotFoundException">If the review is not found.</exception>
    Task<ReviewDto> GetReviewByIdAsync(Guid courseId, Guid reviewId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a review by its ID.
    /// </summary>
    /// <param name="courseId">The ID of the course being reviewed.</param>
    /// <param name="reviewId">The ID of the review to update.</param>
    /// <param name="updateReviewDto">The updated review data (Rating, Comment).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated ReviewDto.</returns>
    /// <exception cref="ServiceNotFoundException">If the course or review is not found.</exception>
    /// <exception cref="ServiceBadRequestException">If validation fails (e.g., user not authorized to update, invalid rating).</exception>
    Task<ReviewDto> UpdateReviewAsync(Guid courseId, Guid reviewId, UpdateReviewDto updateReviewDto, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a review by its ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to delete.</param>
    /// <param name="courseId">The ID of the course being reviewed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ServiceNotFoundException">If the review is not found.</exception>
    /// <exception cref="ServiceAuthorizationException">If the user is not authorized to delete the review.</exception>
    Task DeleteReviewAsync(Guid courseId, Guid reviewId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all reviews for a specified course.
    /// </summary>
    /// <param name="courseId">The ID of the course whose reviews are being retrieved.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of ReviewDto representing the reviews for the course.</returns>
    /// <exception cref="ServiceNotFoundException">If the course is not found.</exception>
    Task<IReadOnlyList<ReviewDto>> GetReviewsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);

    Task<PagedResult<ReviewDto>> GetPagedReviewsByCourseIdAsync(Guid courseId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

