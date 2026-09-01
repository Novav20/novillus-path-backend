namespace SourceGuild.Domain.Entities.Content;

public abstract class ContentBlock
{
    protected ContentBlock() { }

    public Guid Id { get; protected set; }
    public int Order { get; protected set; }
    
    public Guid LessonId { get; protected set; }
    public Lesson Lesson { get; protected set; } = null!;

    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    internal void SetOrder(int newOrder)
    {
        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}