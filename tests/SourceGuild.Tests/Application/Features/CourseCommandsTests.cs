using FluentAssertions;
using Moq;
using SourceGuild.Application.DTOs.Course;
using SourceGuild.Application.DTOs.Section;
using SourceGuild.Application.Features.Courses;
using SourceGuild.Application.Interfaces.Common;
using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Entities;
using SourceGuild.Domain.Enums;
using Xunit;

namespace SourceGuild.Tests.Application.Features;

public class CourseCommandsTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CourseCommands _sut;

    private readonly Guid _instructorId = Guid.NewGuid();

    public CourseCommandsTests()
    {
        _currentUserServiceMock.Setup(u => u.UserId).Returns(_instructorId);

        _sut = new CourseCommands(
            _courseRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenUserNotAuthenticated_ShouldReturnUnauthorizedError()
    {
        // Arrange
        _currentUserServiceMock.Setup(u => u.UserId).Returns((Guid?)null);
        var dto = new CreateCourseDto { Title = "Curso .NET", Price = 10m };

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.Unauthorized");
        _courseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldPersistCourseAndReturnDto()
    {
        // Arrange
        var dto = new CreateCourseDto
        {
            Title = "Diseño de Microservicios",
            Price = 39.99m,
            Description = "Patrones distribuidos"
        };

        _courseRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course c, CancellationToken _) => c);

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be(dto.Title);
        result.Value.Price.Should().Be(dto.Price);
        result.Value.InstructorId.Should().Be(_instructorId);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WhenUserIsNotCourseInstructor_ShouldReturnForbiddenError()
    {
        // Arrange
        var otherInstructorId = Guid.NewGuid();
        var course = Course.Create("Curso de Otro", otherInstructorId, 0m).Value;
        
        _courseRepositoryMock.Setup(r => r.GetWithDetailsAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var result = await _sut.PublishAsync(course.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.Forbidden");
    }

    [Fact]
    public async Task AddSectionAsync_WithValidTitle_ShouldAddSectionThroughAggregateRoot()
    {
        // Arrange
        var course = Course.Create("Curso con Secciones", _instructorId, 0m).Value;
        var dto = new CreateSectionDto { Title = "Módulo 1: Fundamentos" };

        _courseRepositoryMock.Setup(r => r.GetWithDetailsAsync(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var result = await _sut.AddSectionAsync(course.Id, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be(dto.Title);
        result.Value.Order.Should().Be(0);
        course.Sections.Should().HaveCount(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}