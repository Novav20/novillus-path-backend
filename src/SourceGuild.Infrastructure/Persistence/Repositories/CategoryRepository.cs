using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Infrastructure.Persistence.Repositories;

public class CategoryRepository(SGDbContext context) : EfRepository<Category>(context), ICategoryRepository
{
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower().Trim(), cancellationToken);
    }
}