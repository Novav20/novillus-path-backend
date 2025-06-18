using NovillusPath.Application.DTOs.Lesson;

namespace NovillusPath.Application.Interfaces.Services;

public interface ILessonService
{
    Task<IReadOnlyList<LessonDto>> GetLessonsBySectionAsync(Guid sectionId, CancellationToken cancellationToken);
    Task<LessonDto> GetLessonByIdAsync(Guid sectionId, Guid lessonId, CancellationToken cancellationToken);
    Task<LessonDto> CreateLessonAsync(Guid sectionId, CreateLessonDto createLessonDto, CancellationToken cancellationToken);
    Task UpdateLessonAsync(Guid sectionId, Guid lessonId, UpdateLessonDto updateLessonDto, CancellationToken cancellationToken);
    Task DeleteLessonAsync(Guid sectionId, Guid lessonId, CancellationToken cancellationToken);
}
