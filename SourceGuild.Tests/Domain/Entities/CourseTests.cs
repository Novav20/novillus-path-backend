using FluentAssertions;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Tests.Domain.Entities;

public class CourseTests
{
    [Fact]
    public void Course_Price_ShouldNotBeNegative()
    {
        // Arrange
        var course = new Course {Title = "Test Course"};
        const decimal invalidPrice = -10.50m;

        // Act
        course.Price = invalidPrice;

        // Assert
        course.Price.Should().BeGreaterThanOrEqualTo(0,"porque un curso no puede tener un precio negativo");
    }
}
