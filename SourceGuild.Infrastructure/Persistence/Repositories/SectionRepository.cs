namespace SourceGuild.Infrastructure.Persistence.Repositories;

public class SectionRepository(SGDbContext context) : EfRepository<Section>(context), ISectionRepository
{
    public async Task<Section?> GetSectionWithCourseAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        return await _context.Sections
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == sectionId, cancellationToken);
    }

    public async Task<Section?> GetSectionWithLessonsAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        return await _context.Sections
            .Include(s => s.Lessons)
            .FirstOrDefaultAsync(s => s.Id == sectionId, cancellationToken);
    }

    public async Task<Section?> GetFullSectionByIdAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        return await _context.Sections
            .Include(s => s.Course)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.ContentBlocks)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sectionId, cancellationToken);
    }
}
