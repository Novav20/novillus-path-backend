using Moq;
using NovillusPath.Application.Services;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Application.Interfaces.Common;
using AutoMapper;
using NovillusPath.Application.DTOs.Review;
using NovillusPath.Domain.Entities;
using NovillusPath.Application.Exceptions;
using System.Linq.Expressions;

namespace NovillusPath.Test;

public class ReviewServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly ReviewService _reviewService;

    public ReviewServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _reviewService = new ReviewService(_unitOfWorkMock.Object, _mapperMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldThrowServiceBadRequestException_WhenUserIsNotEnrolled()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createReviewDto = new CreateReviewDto { Rating = 5, Comment = "Great course!" };

        _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
        _currentUserServiceMock.Setup(s => s.IsInRole("Student")).Returns(true);

        _unitOfWorkMock.Setup(u => u.CourseRepository.ExistsAsync(It.IsAny<Expression<Func<Course, bool>>>(), default)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.EnrollmentRepository.ExistsAsync(It.IsAny<Expression<Func<Enrollment, bool>>>(), default)).ReturnsAsync(false); // User is not enrolled

        // Act & Assert
        await Assert.ThrowsAsync<ServiceBadRequestException>(() => _reviewService.CreateReviewAsync(courseId, createReviewDto, default));
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldThrowServiceBadRequestException_WhenUserHasAlreadyReviewed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createReviewDto = new CreateReviewDto { Rating = 5, Comment = "Great course!" };

        _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);
        _currentUserServiceMock.Setup(s => s.IsInRole("Student")).Returns(true);

        _unitOfWorkMock.Setup(u => u.CourseRepository.ExistsAsync(It.IsAny<Expression<Func<Course, bool>>>(), default)).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.EnrollmentRepository.ExistsAsync(It.IsAny<Expression<Func<Enrollment, bool>>>(), default)).ReturnsAsync(true); // User is enrolled
        _unitOfWorkMock.Setup(u => u.ReviewRepository.ExistsAsync(It.IsAny<Expression<Func<Review, bool>>>(), default)).ReturnsAsync(true); // User has already reviewed

        // Act & Assert
        await Assert.ThrowsAsync<ServiceBadRequestException>(() => _reviewService.CreateReviewAsync(courseId, createReviewDto, default));
    }

    [Fact]
    public async Task UpdateReviewAsync_ShouldThrowServiceAuthorizationException_WhenUserIsNotOwnerOrAdmin()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid(); // Different user
        var updateReviewDto = new UpdateReviewDto { Rating = 4 };
        var review = new Review { Id = reviewId, CourseId = courseId, UserId = ownerId };

        _currentUserServiceMock.Setup(s => s.UserId).Returns(currentUserId);
        _currentUserServiceMock.Setup(s => s.IsInRole("Admin")).Returns(false); // Not an admin

        _unitOfWorkMock.Setup(u => u.ReviewRepository.GetByIdAsync(reviewId, default)).ReturnsAsync(review);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceAuthorizationException>(() => _reviewService.UpdateReviewAsync(courseId, reviewId, updateReviewDto, default));
    }

    [Fact]
    public async Task DeleteReviewAsync_ShouldThrowServiceAuthorizationException_WhenUserIsNotOwnerOrAdmin()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid(); // Different user
        var review = new Review { Id = reviewId, CourseId = courseId, UserId = ownerId };

        _currentUserServiceMock.Setup(s => s.UserId).Returns(currentUserId);
        _currentUserServiceMock.Setup(s => s.IsInRole("Admin")).Returns(false); // Not an admin

        _unitOfWorkMock.Setup(u => u.ReviewRepository.GetByIdAsync(reviewId, default)).ReturnsAsync(review);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceAuthorizationException>(() => _reviewService.DeleteReviewAsync(reviewId, courseId, default));
    }
}
