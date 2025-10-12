using System.ComponentModel.DataAnnotations;

namespace NovillusPath.Application.DTOs.Course;

public record CreateCourseDto
{
    public required string Title { get; init; }
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public int? DurationInWeeks { get; init; }
    [Url]
    public string? ImageUrl { get; init; }
    public DateTime? StartDate { get; init; }
    public List<Guid>? CategoryIds { get; init; }
}
