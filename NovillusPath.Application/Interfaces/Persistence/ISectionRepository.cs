using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Interfaces.Persistence;

public interface ISectionRepository : IRepository<Section>
{
    Task<Section?> GetSectionWithCourseAsync(Guid sectionId, CancellationToken cancellationToken);
    Task<Section?> GetSectionWithLessonsAsync(Guid sectionId, CancellationToken cancellationToken);
    Task<Section?> GetFullSectionByIdAsync(Guid sectionId, CancellationToken cancellationToken);
}
