namespace NovillusPath.Application.DTOs.Course;

public class CourseSearchParamsDto
{
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public double? MinRating { get; set; }
    public string? SortBy { get; set; } // e.g., "title", "price", "rating", "date"
    public string? SortOrder { get; set; } // "asc" or "desc"
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
