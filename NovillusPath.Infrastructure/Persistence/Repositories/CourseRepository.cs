using Microsoft.EntityFrameworkCore;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class CourseRepository(NovillusDbContext context) : EfRepository<Course>(context), ICourseRepository
{
    public override async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Course>()
            .Include(c => c.Categories)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<Course>> ListAllAsync(CancellationToken cancellationToken = default)
    { 
        return await _context.Set<Course>()
            .Include(c => c.Categories)
            .ToListAsync(cancellationToken);
    }
}
