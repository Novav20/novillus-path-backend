using Microsoft.EntityFrameworkCore;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class LessonRepository(NovillusDbContext context) : EfRepository<Lesson>(context), ILessonRepository
{
    public async Task<IReadOnlyList<Lesson>> GetLessonsBySectionIdAsync(Guid sectionId, bool includeContentBlocks, CancellationToken cancellationToken)
    {
        IQueryable<Lesson> query = _context.Lessons
            .Where(l => l.SectionId == sectionId)
            .OrderBy(l => l.Order);

        if (includeContentBlocks)
        {
            query = query.Include(l => l.ContentBlocks);
        }
        query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Lesson?> GetLessonWithContentBlocksAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        return await _context.Lessons
            .Include(l => l.ContentBlocks)
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);
    }
}
