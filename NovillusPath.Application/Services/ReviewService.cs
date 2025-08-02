using AutoMapper;
using NovillusPath.Application.DTOs.Common;
using NovillusPath.Application.DTOs.Review;
using NovillusPath.Application.Exceptions;
using NovillusPath.Application.Helpers;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Application.Interfaces.Services;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Services;

public class ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : IReviewService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    public async Task<ReviewDto> CreateReviewAsync(Guid courseId, CreateReviewDto createReviewDto, CancellationToken cancellationToken)
    {
        // 1. Authorization
        if (!AuthorizationHelper.CanPerformReviewAction(_currentUserService))
        {
            throw new ServiceAuthorizationException("You are not authorized to submit this review.");
        }
        // 2. Validate Course
        if (!await _unitOfWork.CourseRepository.ExistsAsync(c => c.Id == courseId, cancellationToken))
        {
            throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        }
        // 3. Business Rule: User must be enrolled in the course to review it.
        bool isEnrolled = await _unitOfWork.EnrollmentRepository.ExistsAsync(e => e.UserId == _currentUserService.UserId && e.CourseId == courseId, cancellationToken);
        if (!isEnrolled)
        {
            throw new ServiceBadRequestException("You must be enrolled in the course to submit a review.");
        }
        // 4. Business Rule: A user can only submit one review per course.
        bool alreadyReviewed = await _unitOfWork.ReviewRepository.ExistsAsync(r => r.UserId == _currentUserService.UserId && r.CourseId == courseId, cancellationToken);
        if (alreadyReviewed)
        {
            throw new ServiceBadRequestException("You have already submitted a review for this course.");
        }
        // 5. Create Review
        var review = _mapper.Map<Review>(createReviewDto);
        review.UserId = _currentUserService.UserId!.Value;
        review.CourseId = courseId;

        await _unitOfWork.ReviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var reviewWithDetails = await _unitOfWork.ReviewRepository.GetReviewDetailsByIdAsync(review.Id, cancellationToken) ?? throw new ServiceNotFoundException($"Newly created review with ID {review.Id} could not be retrieved.");

        return _mapper.Map<ReviewDto>(reviewWithDetails, opts => opts.Items["currentUserService"] = _currentUserService);
    }

    public async Task<ReviewDto> GetReviewByIdAsync(Guid courseId, Guid reviewId, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.ReviewRepository.GetReviewDetailsByIdAsync(reviewId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Review with ID {reviewId} not found.");

        if (review.CourseId != courseId)
        {
            throw new ServiceNotFoundException($"Review with ID {reviewId} not found in course {courseId}.");
        }
        // Optional: Add visibility rules here too (e.g., only published reviews visible to public)
        return _mapper.Map<ReviewDto>(review, opts => opts.Items["currentUserService"] = _currentUserService);
    }

    public async Task<ReviewDto> UpdateReviewAsync(Guid courseId, Guid reviewId, UpdateReviewDto updateReviewDto, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.ReviewRepository.GetByIdAsync(reviewId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Review with ID {reviewId} not found.");

        if (review.CourseId != courseId)
        {
            throw new ServiceNotFoundException($"Review with ID {reviewId} not found in course {courseId}.");
        }

        if (!AuthorizationHelper.CanModifyReview(_currentUserService, review.UserId))
        {
            throw new ServiceAuthorizationException("You are not authorized to update this review.");
        }

        _mapper.Map(updateReviewDto, review);
        review.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedReviewWithDetails = await _unitOfWork.ReviewRepository.GetReviewDetailsByIdAsync(reviewId, cancellationToken)
            ?? throw new Exception($"Failed to retrieve updated review details for ID {reviewId}.");

        return _mapper.Map<ReviewDto>(updatedReviewWithDetails, opts => opts.Items["currentUserService"] = _currentUserService);
    }

    public async Task DeleteReviewAsync(Guid reviewId, Guid courseId, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.ReviewRepository.GetByIdAsync(reviewId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Review with ID {reviewId} not found.");

        if (review.CourseId != courseId)
        {
            throw new ServiceNotFoundException($"Review with ID {reviewId} not found in course {courseId}.");
        }

        if (!AuthorizationHelper.CanModifyReview(_currentUserService, review.UserId))
        {
            throw new ServiceAuthorizationException("You are not authorized to delete this review.");
        }

        await _unitOfWork.ReviewRepository.DeleteAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReviewDto>> GetReviewsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");

        if (!VisibilityHelper.CanUserViewReviewListForCourse(course, _currentUserService))
        {
            throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        }

        var reviews = await _unitOfWork.ReviewRepository.GetReviewsByCourseIdWithUserDetailsAsync(courseId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews, opts => opts.Items["currentUserService"] = _currentUserService);
    }

    public async Task<PagedResult<ReviewDto>> GetPagedReviewsByCourseIdAsync(
        Guid courseId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        // 1. Validate course existence and review visibility for the current user
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");

        if (!VisibilityHelper.CanUserViewReviewListForCourse(course, _currentUserService))
        {
            throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        }

        // 2. Retrieve paginated reviews and the total count from the repository
        var (reviews, totalCount) = await _unitOfWork.ReviewRepository
            .GetPagedReviewsByCourseIdWithUserDetailsAsync(courseId, pageNumber, pageSize, cancellationToken);

        // 3. Map the list of Review entities to ReviewDto
        var reviewDtos = _mapper.Map<IReadOnlyList<ReviewDto>>(reviews, opts => opts.Items["currentUserService"] = _currentUserService);

        // 4. Calculate the total number of pages
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        // 5. Build and return the paginated result
        return new PagedResult<ReviewDto>
        {
            Items = reviewDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }
}


