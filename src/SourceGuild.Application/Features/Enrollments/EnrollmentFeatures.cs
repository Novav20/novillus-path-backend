using SourceGuild.Application.Common.Mappings;
using SourceGuild.Application.DTOs.Dashboard;
using SourceGuild.Application.Interfaces.Common;
using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Common;
using SourceGuild.Domain.Entities;
using SourceGuild.Domain.Enums;

namespace SourceGuild.Application.Features.Enrollments;

public class EnrollmentFeatures(
    IEnrollmentRepository enrollmentRepository,
    ICourseRepository courseRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid>> EnrollAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        if (!currentUserService.UserId.HasValue || currentUserService.UserId.Value == Guid.Empty)
            return Result<Guid>.Failure(Error.Validation("Auth.Unauthorized", "Usuario no autenticado."));

        var userId = currentUserService.UserId.Value;

        var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null)
            return Result<Guid>.Failure(Error.NotFound("Course.NotFound", "El curso no existe."));

        if (course.Status != CourseStatus.Published)
            return Result<Guid>.Failure(Error.Validation("Course.NotPublished", "No es posible matricularse en un curso que no está publicado."));

        var existingEnrollment = await enrollmentRepository.GetByUserAndCourseAsync(userId, courseId, cancellationToken);
        if (existingEnrollment is not null)
            return Result<Guid>.Failure(Error.Conflict("Enrollment.AlreadyEnrolled", "El usuario ya se encuentra matriculado en este curso."));

        var enrollmentResult = Enrollment.Create(userId, courseId);
        if (enrollmentResult.IsFailure)
            return Result<Guid>.Failure(enrollmentResult.Error);

        var enrollment = await enrollmentRepository.AddAsync(enrollmentResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(enrollment.Id);
    }

    public async Task<Result> UpdateProgressAsync(Guid courseId, int progressPercentage, CancellationToken cancellationToken = default)
    {
        if (!currentUserService.UserId.HasValue || currentUserService.UserId.Value == Guid.Empty)
            return Result.Failure(Error.Validation("Auth.Unauthorized", "Usuario no autenticado."));

        var userId = currentUserService.UserId.Value;

        var enrollment = await enrollmentRepository.GetByUserAndCourseAsync(userId, courseId, cancellationToken);
        if (enrollment is null)
            return Result.Failure(Error.NotFound("Enrollment.NotFound", "El registro de matrícula no existe."));

        var progressResult = enrollment.UpdateProgress(progressPercentage);
        if (progressResult.IsFailure)
            return progressResult;

        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<IReadOnlyList<EnrolledCourseSummaryDto>> GetStudentEnrollmentsAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUserService.UserId.HasValue || currentUserService.UserId.Value == Guid.Empty) 
            return [];

        var userId = currentUserService.UserId.Value;

        var enrollments = await enrollmentRepository.GetByUserIdAsync(userId, includeCourseDetails: true, cancellationToken);
        return enrollments.Select(e => e.ToEnrolledSummaryDto()).ToList();
    }
}