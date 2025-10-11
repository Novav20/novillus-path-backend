namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class CategoryRepository(NovillusDbContext context) : EfRepository<Category>(context), ICategoryRepository
{
    
}
