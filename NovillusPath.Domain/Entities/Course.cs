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
    [ForeignKey(nameof(Instructor))]
    public Guid InstructorId { get; set; }
    public ApplicationUser? Instructor { get; set; }
    public ICollection<Category> Categories { get; set; } = [];
    public ICollection<Section> Sections { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];

}