using NovillusPath.Application.DTOs.ContentBlock;

namespace NovillusPath.Application.DTOs.Lesson;

public record LessonDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public int Order { get; init; }
    public required string Status { get; init; }
    public Guid SectionId { get; init; }
    public ICollection<ContentBlockDto> ContentBlocks { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
