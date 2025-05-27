using NovillusPath.Domain.Enums;

namespace NovillusPath.Application.DTOs.Course;

public class UpdateCourseDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public CourseStatus? Status { get; set; }
    public int? DurationInWeeks { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
}
