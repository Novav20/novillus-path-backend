namespace NovillusPath.Application.Interfaces.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    ICourseRepository CourseRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    ISectionRepository SectionRepository { get; }
    ILessonRepository LessonRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}