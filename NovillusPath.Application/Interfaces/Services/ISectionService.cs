using NovillusPath.Application.DTOs.Section;

namespace NovillusPath.Application.Interfaces.Services
{
    public interface ISectionService
    {
        Task<IReadOnlyList<SectionDto>> GetSectionsAsync(Guid courseId, CancellationToken cancellationToken);
        Task<SectionDto> GetSectionByIdAsync(Guid courseId, Guid sectionId, CancellationToken cancellationToken);
        Task<SectionDto> CreateSectionAsync(Guid courseId, CreateSectionDto createSectionDto, CancellationToken cancellationToken);
        Task UpdateSectionAsync(Guid courseId, Guid sectionId, UpdateSectionDto updateSectionDto, CancellationToken cancellationToken);
        Task DeleteSectionAsync(Guid courseId, Guid sectionId, CancellationToken cancellationToken);
        Task UpdateSectionStatusAsync(Guid courseId, Guid sectionId, UpdateSectionStatusDto updateStatusDto, CancellationToken cancellationToken);
    }
}
