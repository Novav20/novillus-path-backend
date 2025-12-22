namespace SourceGuild.Application.DTOs.Course;

public record CourseSearchParamsDto
{
    public string? SearchTerm { get; init; }
    public Guid? CategoryId { get; init; }
    public double? MinRating { get; init; }
    public string? SortBy { get; init; } // e.g., "title", "price", "rating", "date"
    public string? SortOrder { get; init; } // "asc" or "desc"
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
