namespace NovillusPath.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    ICourseRepository CourseRepository { get; } // Expose specific repositories
    // ICategoryRepository CategoryRepository { get; } // Example for later
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}