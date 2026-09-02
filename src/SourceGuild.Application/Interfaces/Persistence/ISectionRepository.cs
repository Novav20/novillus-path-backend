using SourceGuild.Domain.Entities;

namespace SourceGuild.Application.Interfaces.Persistence;

public interface ISectionRepository : IRepository<Section>
{
    Task<Section?> GetWithLessonsAsync(Guid sectionId, CancellationToken cancellationToken = default);
}