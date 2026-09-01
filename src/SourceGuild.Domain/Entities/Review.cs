using SourceGuild.Domain.Common;

namespace SourceGuild.Domain.Entities;

public class Review
{
    private Review() { }

    public static Result<Review> Create(Guid userId, Guid courseId, byte rating, string? comment = null)
    {
        if (rating is < 1 or > 5)
            return Result<Review>.Failure(Error.Validation("Review.InvalidRating", "La calificación debe estar entre 1 y 5 estrellas."));

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            Rating = rating,
            Comment = comment?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return Result<Review>.Success(review);
    }

    public Guid Id { get; private set; }
    public byte Rating { get; private set; }
    public string? Comment { get; private set; }

    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; } = null!;

    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Result Update(byte rating, string? comment)
    {
        if (rating is < 1 or > 5)
            return Result.Failure(Error.Validation("Review.InvalidRating", "La calificación debe estar entre 1 y 5 estrellas."));

        Rating = rating;
        Comment = comment?.Trim();
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}