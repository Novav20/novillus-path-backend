using FluentAssertions;
using Moq;
using SourceGuild.Application.Features.Enrollments;
using SourceGuild.Application.Interfaces.Common;
using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Entities;
using SourceGuild.Domain.Enums;
using Xunit;

namespace SourceGuild.Tests.Application.Features;

public class EnrollmentFeaturesTests
{
    private readonly Mock<IEnrollmentRepository> _enrollmentRepositoryMock = new();
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly EnrollmentFeatures _sut;

    private readonly Guid _studentId = Guid.NewGuid();

    public EnrollmentFeaturesTests()
    {
        _currentUserServiceMock.Setup(u => u.UserId).Returns(_studentId);

        _sut = new EnrollmentFeatures(
            _enrollmentRepositoryMock.Object,
            _courseRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task EnrollAsync_WhenCourseIsNotPublished_ShouldReturnValidationError()
    {
        // Arrange
        var course = Course.Create("Curso en Borrador", Guid.NewGuid(), 0m).Value; // Estado: Draft por defecto
        
        _courseRepositoryMock.Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var result = await _sut.EnrollAsync(course.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Course.NotPublished");
        _enrollmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task EnrollAsync_WhenAlreadyEnrolled_ShouldReturnConflictError()
    {
        // Arrange
        var course = Course.Create("Curso Publicado", Guid.NewGuid(), 0m).Value;
        var section = course.AddSection("Módulo").Value;
        section.AddLesson("Lección");
        course.Publish();

        var existingEnrollment = Enrollment.Create(_studentId, course.Id).Value;

        _courseRepositoryMock.Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _enrollmentRepositoryMock.Setup(r => r.GetByUserAndCourseAsync(_studentId, course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEnrollment);

        // Act
        var result = await _sut.EnrollAsync(course.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Enrollment.AlreadyEnrolled");
    }

    [Fact]
    public async Task UpdateProgressAsync_WithInvalidPercentage_ShouldReturnValidationError()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var enrollment = Enrollment.Create(_studentId, courseId).Value;

        _enrollmentRepositoryMock.Setup(r => r.GetByUserAndCourseAsync(_studentId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        // Act (Porcentaje inválido > 100)
        var result = await _sut.UpdateProgressAsync(courseId, 150);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Enrollment.InvalidProgress");
        enrollment.ProgressPercentage.Should().Be(0);
    }
}