using NovillusPath.Application.DTOs.Category;

namespace NovillusPath.Application.DTOs.Course;

public record CourseListProjectionDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public required string Status { get; init; } 
    public int? DurationInWeeks { get; init; }
    public string? ImageUrl { get; init; }
    public DateTime? StartDate { get; init; }
    public Guid InstructorId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public List<CategoryListItemDto> Categories { get; init; } = [];

    public int TotalRatings { get; init; }
    public double AverageRating { get; init; }
}