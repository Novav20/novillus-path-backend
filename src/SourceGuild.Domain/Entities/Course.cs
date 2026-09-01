namespace SourceGuild.Domain.Entities;

public class Course
{
    private decimal _price;

    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("El precio de un curso no puede ser negativo.", nameof(value));
            }
            _price = value;
        }
    }

    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public int? DurationInWeeks { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid InstructorId { get; set; }
    public ApplicationUser? Instructor { get; set; }

    public ICollection<Category> Categories { get; set; } = [];
    public ICollection<Section> Sections { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}