using System.Linq.Expressions;

namespace NovillusPath.Application.Interfaces.Persistence;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<Lesson?> GetLessonWithContentBlocksAsync(Guid lessonId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Lesson>> GetLessonsBySectionIdAsync(Guid sectionId, bool includeContentBlocks, CancellationToken cancellationToken);
    Task<IReadOnlyList<Lesson>> GetFilteredLessonsAsync(
    Expression<Func<Lesson, bool>> filterPredicate,
    bool includeContentBlocks,
    CancellationToken cancellationToken);
}
