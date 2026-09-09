using FluentAssertions;
using Moq;
using SourceGuild.Application.DTOs.Category;
using SourceGuild.Application.Features.Categories;
using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Entities;
using Xunit;

namespace SourceGuild.Tests.Application.Features;

public class CategoryFeaturesTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CategoryFeatures _sut; // System Under Test

    public CategoryFeaturesTests()
    {
        _sut = new CategoryFeatures(_categoryRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldPersistAndReturnSuccess()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            Name = "Arquitectura de Software",
            Description = "Cursos avanzados de diseño de sistemas"
        };

        _categoryRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(dto.Name);
        result.Value.Description.Should().Be(dto.Description);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ShouldReturnValidationError()
    {
        // Arrange
        var dto = new CreateCategoryDto { Name = "   " };

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Category.EmptyName");
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _sut.GetByIdAsync(categoryId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Category.NotFound");
    }
}