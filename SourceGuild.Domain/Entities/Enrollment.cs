namespace SourceGuild.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(Course))]
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public int ProgressPercentage { get; set; } = 0;
}
