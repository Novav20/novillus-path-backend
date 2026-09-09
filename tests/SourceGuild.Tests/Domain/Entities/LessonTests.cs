using FluentAssertions;
using SourceGuild.Domain.Entities;
using SourceGuild.Domain.Enums;
using Xunit;

namespace SourceGuild.Tests.Domain.Entities;

public class LessonTests
{
    [Fact]
    public void AddLesson_ThroughSection_ShouldHaveDefaultDraftStatusAndSequentialOrder()
    {
        // Arrange
        var course = Course.Create("Dominio de C#", Guid.NewGuid(), 0).Value;
        var section = course.AddSection("Módulo 1").Value;
        const string lessonTitle = "Introducción a Tipos y Variables";

        // Act
        var lessonResult = section.AddLesson(lessonTitle);

        // Assert
        lessonResult.IsSuccess.Should().BeTrue();
        var lesson = lessonResult.Value;
        lesson.Title.Should().Be(lessonTitle);
        lesson.Status.Should().Be(LessonStatus.Draft);
        lesson.Order.Should().Be(0);
        lesson.Id.Should().NotBeEmpty();
        lesson.ContentBlocks.Should().BeEmpty();
    }

    [Fact]
    public void AddTextContent_ShouldAppendContentBlockToLesson()
    {
        // Arrange
        var course = Course.Create("Dominio de C#", Guid.NewGuid(), 0).Value;
        var section = course.AddSection("Módulo 1").Value;
        var lesson = section.AddLesson("Lección 1").Value;
        const string markdownText = "# Resumen de la Lección\nContenido teórico.";

        // Act
        var contentResult = lesson.AddTextContent(markdownText);

        // Assert
        contentResult.IsSuccess.Should().BeTrue();
        contentResult.Value.Text.Should().Be(markdownText);
        contentResult.Value.Order.Should().Be(0);
        lesson.ContentBlocks.Should().HaveCount(1);
    }

    [Fact]
    public void AddVideoContent_WithNegativeDuration_ShouldFailValidation()
    {
        // Arrange
        var course = Course.Create("Dominio de C#", Guid.NewGuid(), 0).Value;
        var section = course.AddSection("Módulo 1").Value;
        var lesson = section.AddLesson("Lección 1").Value;

        // Act
        var videoResult = lesson.AddVideoContent("https://cdn.sourceguild.com/video1.mp4", durationMinutes: -5);

        // Assert
        videoResult.IsFailure.Should().BeTrue();
        videoResult.Error.Code.Should().Be("VideoContent.InvalidDuration");
        lesson.ContentBlocks.Should().BeEmpty();
    }
}