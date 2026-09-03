using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Infrastructure.Persistence.Repositories;

public class SectionRepository(SGDbContext context) : EfRepository<Section>(context), ISectionRepository
{
    public async Task<Section?> GetWithLessonsAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        return await _context.Sections
            .Include(s => s.Lessons)
            .FirstOrDefaultAsync(s => s.Id == sectionId, cancellationToken);
    }
}