using System;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Interfaces.Persistence;

public interface ICourseRepository : IRepository<Course>
{
    // Example of a Course-specific method we might need later:
    // Task<Course?> GetCourseWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    // Task<IReadOnlyList<Course>> GetCoursesByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default);
}
