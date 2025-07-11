namespace NovillusPath.Application.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; init; }
    public byte Rating { get; init; }
    public string? Comment { get; init; }
    public required string UserFullName { get; init; }
    public string? UserProfileImageUrl { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid CourseId { get; init; }
    public Guid UserId { get; init; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}
