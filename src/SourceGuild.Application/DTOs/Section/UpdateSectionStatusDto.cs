namespace SourceGuild.Application.DTOs.Section;

public record UpdateSectionStatusDto
{
    public required string Status { get; init; }
}