using SourceGuild.Domain.Entities;

namespace SourceGuild.Application.Interfaces.Persistence;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<Lesson?> GetWithContentBlocksAsync(Guid lessonId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lesson>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
}