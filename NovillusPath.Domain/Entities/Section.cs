using System.ComponentModel.DataAnnotations.Schema;
using NovillusPath.Domain.Enums;

namespace NovillusPath.Domain.Entities;

public class Section
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; set; }
    public int Order { get; set; } = default; // Default to 0 (the only one section that unenrolled students can see: the overview of the course)
    public SectionStatus Status { get; set; } = SectionStatus.Draft;
    [ForeignKey(nameof(Course))]
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Lesson> Lessons { get; set; } = [];
}
