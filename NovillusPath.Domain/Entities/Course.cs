using NovillusPath.Domain.Enums;

namespace NovillusPath.Domain.Entities;

public class Course
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public int? DurationInWeeks { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid InstructorId { get; init; } 
    public ICollection<Category> Categories { get; set; } = [];
}
