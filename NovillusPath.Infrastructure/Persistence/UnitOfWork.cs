using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Infrastructure.Persistence.Repositories;

namespace NovillusPath.Infrastructure.Persistence;

public class UnitOfWork(NovillusDbContext context) : IUnitOfWork
{
    private readonly NovillusDbContext _context = context;
    // Lazy load repositories
    private ICourseRepository? _courseRepository;
    private ICategoryRepository? _categoryRepository;
    private ISectionRepository? _sectionRepository;
    private ILessonRepository? _lessonRepository;
    private IEnrollmentRepository? _enrollmentRepository;

    public ICourseRepository CourseRepository =>
        _courseRepository ??= new CourseRepository(_context);

    public ICategoryRepository CategoryRepository =>
        _categoryRepository ??= new CategoryRepository(_context);

    public ISectionRepository SectionRepository =>
        _sectionRepository ??= new SectionRepository(_context);

    public ILessonRepository LessonRepository =>
        _lessonRepository ??= new LessonRepository(_context);

    public IEnrollmentRepository EnrollmentRepository =>
        _enrollmentRepository ??= new EnrollmentRepository(_context);

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }


}
