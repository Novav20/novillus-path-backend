using NovillusPath.Application.DTOs.Lesson;

namespace NovillusPath.Application.DTOs.Section;

public class SectionDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public int Order { get; init; }
    public required string Status { get; init; }
    public DateTime CreatedAt { get; init; } 
    public DateTime UpdatedAt { get; init; }
    public IReadOnlyList<LessonDto>? Lessons { get; init; } = []; 
}
