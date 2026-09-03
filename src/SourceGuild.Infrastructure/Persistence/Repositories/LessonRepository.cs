using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Infrastructure.Persistence.Repositories;

public class LessonRepository(SGDbContext context) : EfRepository<Lesson>(context), ILessonRepository
{
    public async Task<Lesson?> GetWithContentBlocksAsync(Guid lessonId, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons
            .Include(l => l.ContentBlocks)
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);
    }

    public async Task<IReadOnlyList<Lesson>> GetBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons
            .Where(l => l.SectionId == sectionId)
            .OrderBy(l => l.Order)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}