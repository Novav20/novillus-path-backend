namespace NovillusPath.Application.DTOs.Section;

public class SectionDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public int Order { get; init; }
    public DateTime CreatedAt { get; init; } 
    public DateTime UpdatedAt { get; init; }
    // public List<LessonDto> Lessons { get; init; } = []; // For Day 2
}
