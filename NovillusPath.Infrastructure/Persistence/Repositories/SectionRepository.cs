using System;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class SectionRepository(NovillusDbContext context) : EfRepository<Section>(context), ISectionRepository
{

}
