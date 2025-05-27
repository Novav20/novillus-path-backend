using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class CourseRepository(NovillusDbContext context) : EfRepository<Course>(context), ICourseRepository
{
}
