using System;

namespace NovillusPath.Application.Interfaces.Services;

public interface IEnrollmentService
{
    /// <summary>
    /// Enrolls a specified user in a specified course.
    /// </summary>
    /// <param name="courseId">The ID of the course to enroll in.</param>
    /// <param name="userId">The ID of the user to enroll.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ServiceNotFoundException">If the course or user is not found.</exception>
    /// <exception cref="ServiceBadRequestException">If the user is already enrolled, or if the course is not publish for enrollment.</exception>
    /// <exception cref="ServiceAuthorizationException">If the user is not authorized to enroll (e.g., not a student).</exception>
    Task EnrollAsync(Guid courseId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Unenrolls a specified user from a specified course.
    /// </summary>
    /// <param name="courseId">The ID of the course to unenroll from.</param>
    /// <param name="userId">The ID of the user to unenroll.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ServiceNotFoundException">If the enrollment does not exist.</exception>
    /// <exception cref="ServiceAuthorizationException">If the user is not authorized to unenroll (e.g., trying to unenroll someone else).</exception>
    Task UnenrollAsync(Guid courseId, Guid userId, CancellationToken cancellationToken);
}
