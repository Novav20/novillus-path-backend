using SourceGuild.Domain.Entities;

namespace SourceGuild.Application.Interfaces.Persistence;

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}