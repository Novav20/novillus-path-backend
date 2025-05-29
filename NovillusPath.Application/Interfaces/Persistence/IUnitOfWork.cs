namespace NovillusPath.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    ICourseRepository CourseRepository { get; } 
    ICategoryRepository CategoryRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}