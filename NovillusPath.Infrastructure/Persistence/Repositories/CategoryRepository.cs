using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class CategoryRepository(NovillusDbContext context) : EfRepository<Category>(context), ICategoryRepository
{
    
}
