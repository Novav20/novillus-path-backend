using Microsoft.EntityFrameworkCore;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class SectionRepository(NovillusDbContext context) : EfRepository<Section>(context), ISectionRepository
{
    public async Task<Section?> GetSectionWithCourseAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        return await _context.Sections
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == sectionId, cancellationToken);
    }
}
