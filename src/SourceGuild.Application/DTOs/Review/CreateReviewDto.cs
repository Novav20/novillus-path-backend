namespace SourceGuild.Application.DTOs.Review;

public record CreateReviewDto
{
    public byte Rating { get; init; }
    public string? Comment { get; init; }
}
