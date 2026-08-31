namespace SourceGuild.Application.DTOs.Category;

public record CreateCategoryDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
