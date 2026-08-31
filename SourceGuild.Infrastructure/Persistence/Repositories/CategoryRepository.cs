namespace SourceGuild.Infrastructure.Persistence.Repositories;

public class CategoryRepository(SGDbContext context) : EfRepository<Category>(context), ICategoryRepository
{
    
}
