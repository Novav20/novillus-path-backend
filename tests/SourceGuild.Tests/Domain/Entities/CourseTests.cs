using FluentAssertions;
using SourceGuild.Domain.Entities;
using SourceGuild.Domain.Enums;

namespace SourceGuild.Tests.Domain.Entities;

public class CourseTests
{
    private readonly Guid _instructorId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        const string title = "Arquitectura Limpia en .NET 10";
        const decimal price = 49.99m;

        // Act
        var result = Course.Create(title, _instructorId, price);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.Title.Should().Be(title);
        result.Value.Price.Should().Be(price);
        result.Value.Status.Should().Be(CourseStatus.Draft);
        result.Value.Sections.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldReturnFailureResult()
    {
        // Arrange
        const decimal negativePrice = -10.50m;

        // Act
        var result = Course.Create("Curso Inválido", _instructorId, negativePrice);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Course.NegativePrice");
    }

    [Fact]
    public void UpdatePrice_WithNegativeValue_ShouldFailAndPreserveOriginalPrice()
    {
        // Arrange
        var course = Course.Create("Patrones de Diseño", _instructorId, 25.00m).Value;

        // Act
        var updateResult = course.UpdatePrice(-5.00m);

        // Assert
        updateResult.IsFailure.Should().BeTrue();
        updateResult.Error.Code.Should().Be("Course.NegativePrice");
        course.Price.Should().Be(25.00m);
    }

    [Fact]
    public void Publish_WithoutSections_ShouldFailValidation()
    {
        // Arrange
        var course = Course.Create("Curso sin Secciones", _instructorId, 0).Value;

        // Act
        var publishResult = course.Publish();

        // Assert
        publishResult.IsFailure.Should().BeTrue();
        publishResult.Error.Code.Should().Be("Course.NoSections");
        course.Status.Should().Be(CourseStatus.Draft);
    }

    [Fact]
    public void Publish_WithValidSectionsAndLessons_ShouldTransitionToPublished()
    {
        // Arrange
        var course = Course.Create("Curso Completo", _instructorId, 19.99m).Value;
        var section = course.AddSection("Módulo 1: Fundamentos").Value;
        section.AddLesson("Lección 1: Introducción");

        // Act
        var publishResult = course.Publish();

        // Assert
        publishResult.IsSuccess.Should().BeTrue();
        course.Status.Should().Be(CourseStatus.Published);
    }

    [Fact]
    public void AddSection_ShouldMaintainSequentialOrderWithoutGaps()
    {
        // Arrange
        var course = Course.Create("Curso con Múltiples Secciones", _instructorId, 0).Value;

        // Act
        var sec1 = course.AddSection("Sección 1").Value;
        var sec2 = course.AddSection("Sección 2").Value;
        var sec3 = course.AddSection("Sección 3").Value;

        // Assert
        sec1.Order.Should().Be(0);
        sec2.Order.Should().Be(1);
        sec3.Order.Should().Be(2);
        course.Sections.Should().HaveCount(3);
    }

    [Fact]
    public void RemoveSection_ShouldReindexRemainingSections()
    {
        // Arrange
        var course = Course.Create("Curso Reindexado", _instructorId, 0).Value;
        var sec1 = course.AddSection("Sección 1").Value;
        var sec2 = course.AddSection("Sección 2").Value;
        var sec3 = course.AddSection("Sección 3").Value;

        // Act (Eliminar la del medio: sec2 con Order 1)
        var removeResult = course.RemoveSection(sec2.Id);

        // Assert
        removeResult.IsSuccess.Should().BeTrue();
        course.Sections.Should().HaveCount(2);
        
        var remainingSections = course.Sections.ToList();
        remainingSections[0].Id.Should().Be(sec1.Id);
        remainingSections[0].Order.Should().Be(0);

        remainingSections[1].Id.Should().Be(sec3.Id);
        remainingSections[1].Order.Should().Be(1); // Re-indexada de 2 a 1 automáticamente
    }
}