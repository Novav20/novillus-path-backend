namespace SourceGuild.Domain.Entities;

public class Lesson
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [ForeignKey(nameof(Section))]
    public Guid SectionId { get; set; }
    public Section Section { get; set; } = null!;
    public required string Title { get; set; }
    public int Order { get; set; }
    public LessonStatus Status { get; set; } = LessonStatus.Draft;
    public ICollection<ContentBlock> ContentBlocks { get; set; } = [];
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
