namespace NovillusPath.Application.DTOs.Section;

public record CreateSectionDto
{
    public required string Title { get; init; }
    public int? Order { get; init; } // allowing for server-side logic to determine the order if not provided.
}
