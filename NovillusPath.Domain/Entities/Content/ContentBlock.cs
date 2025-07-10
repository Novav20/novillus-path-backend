namespace NovillusPath.Domain.Entities.Content;

public abstract class ContentBlock
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Order { get; set; }
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
