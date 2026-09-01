using SourceGuild.Domain.Common;

namespace SourceGuild.Domain.Entities;

public class Enrollment
{
    private Enrollment() { }

    public static Result<Enrollment> Create(Guid userId, Guid courseId)
    {
        if (userId == Guid.Empty)
            return Result<Enrollment>.Failure(Error.Validation("Enrollment.InvalidUser", "El identificador de usuario es inválido."));

        if (courseId == Guid.Empty)
            return Result<Enrollment>.Failure(Error.Validation("Enrollment.InvalidCourse", "El identificador de curso es inválido."));

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            ProgressPercentage = 0
        };

        return Result<Enrollment>.Success(enrollment);
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; } = null!;

    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;

    public DateTime EnrolledAt { get; private set; }
    public int ProgressPercentage { get; private set; }

    public Result UpdateProgress(int percentage)
    {
        if (percentage is < 0 or > 100)
            return Result.Failure(Error.Validation("Enrollment.InvalidProgress", "El porcentaje de progreso debe estar entre 0 y 100."));

        ProgressPercentage = percentage;
        return Result.Success();
    }
}