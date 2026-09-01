namespace SourceGuild.Application.DTOs.Common;

/// <summary>
/// Represents a paginated result set.
/// </summary>
/// <typeparam name="T">The type of items in the result set.</typeparam>
public record PagedResult<T> where T : class
{
    /// <summary>
    /// The items for the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = new List<T>();

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// The size of each page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages { get; init; }
}