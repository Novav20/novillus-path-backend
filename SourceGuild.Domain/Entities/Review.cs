namespace SourceGuild.Domain.Entities;

public class Review
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public byte Rating { get; set; } // e.g., 1-5 stars
    public string? Comment { get; set; } // Nullable if comments are optional

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key to User
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    // Foreign Key to Course
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
}
