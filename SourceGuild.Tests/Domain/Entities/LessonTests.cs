using FluentAssertions;
using Xunit;
using SourceGuild.Domain.Entities;
using SourceGuild.Domain.Enums;

namespace SourceGuild.Tests.Domain.Entities;

public class LessonTests
{
    [Fact]
    public void Lesson_Should_HaveDefaultDraftStatus_WhenCreated(){
       // 1. Arrange
       var lessonTitle = "C# para retrasados";

       // 2. Act
       var lesson = new Lesson {Title = lessonTitle}; 

       // 3. Assert
       lesson.Status.Should().Be(LessonStatus.Draft);
       lesson.Title.Should().Be(lessonTitle);
       lesson.Id.Should().NotBeEmpty();
    }
}
