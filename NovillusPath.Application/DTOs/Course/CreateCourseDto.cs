namespace NovillusPath.Application.DTOs.Course;

public class CreateCourseDto
{
    public required string Title { get; set; }
    public decimal Price { get; set; }
    public Guid InstructorId { get; init; } // TODO: we'll get this from the authenticated user.
    public string? Description { get; set; }
    public int? DurationInWeeks { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
}
