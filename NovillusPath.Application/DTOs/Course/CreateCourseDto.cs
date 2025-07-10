using System.ComponentModel.DataAnnotations;

namespace NovillusPath.Application.DTOs.Course;

public class CreateCourseDto
{
    public required string Title { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int? DurationInWeeks { get; set; }
    [Url]
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public List<Guid>? CategoryIds { get; set; }
}
