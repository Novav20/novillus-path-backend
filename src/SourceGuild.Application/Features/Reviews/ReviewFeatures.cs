using SourceGuild.Application.Common.Mappings;
using SourceGuild.Application.DTOs.Review;
using SourceGuild.Application.Interfaces.Common;
using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Common;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Application.Features.Reviews;

public class ReviewFeatures(
    IReviewRepository reviewRepository,
    ICourseRepository courseRepository,
    IEnrollmentRepository enrollmentRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<ReviewDto>> AddReviewAsync(Guid courseId, CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        if (!currentUserService.UserId.HasValue || currentUserService.UserId.Value == Guid.Empty)
            return Result<ReviewDto>.Failure(Error.Validation("Auth.Unauthorized", "Usuario no autenticado."));

        var userId = currentUserService.UserId.Value;

        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
            return Result<ReviewDto>.Failure(Error.NotFound("Course.NotFound", "El curso no existe."));

        var enrollment = await enrollmentRepository.GetByUserAndCourseAsync(userId, courseId, cancellationToken);
        if (enrollment is null)
            return Result<ReviewDto>.Failure(Error.Validation("Review.NotEnrolled", "Solo los estudiantes matriculados pueden calificar este curso."));

        var reviewResult = Review.Create(userId, courseId, dto.Rating, dto.Comment);
        if (reviewResult.IsFailure)
            return Result<ReviewDto>.Failure(reviewResult.Error);

        var review = await reviewRepository.AddAsync(reviewResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ReviewDto>.Success(review.ToDto(userId));
    }

    public async Task<Result> UpdateReviewAsync(Guid reviewId, UpdateReviewDto dto, CancellationToken cancellationToken = default)
    {
        if (!currentUserService.UserId.HasValue || currentUserService.UserId.Value == Guid.Empty)
            return Result.Failure(Error.Validation("Auth.Unauthorized", "Usuario no autenticado."));

        var userId = currentUserService.UserId.Value;

        var review = await reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review is null)
            return Result.Failure(Error.NotFound("Review.NotFound", "La reseña no existe."));

        if (review.UserId != userId)
            return Result.Failure(Error.Validation("Auth.Forbidden", "No tiene permisos para modificar esta reseña."));

        var updateResult = review.Update(dto.Rating ?? review.Rating, dto.Comment ?? review.Comment);
        if (updateResult.IsFailure)
            return updateResult;

        await reviewRepository.UpdateAsync(review, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<IReadOnlyList<ReviewDto>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var reviews = await reviewRepository.GetByCourseIdAsync(courseId, cancellationToken);
        var currentUserId = currentUserService.UserId;
        return reviews.Select(r => r.ToDto(currentUserId)).ToList();
    }
}