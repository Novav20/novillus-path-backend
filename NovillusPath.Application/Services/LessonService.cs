using AutoMapper;
using NovillusPath.Application.DTOs.Lesson;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Application.Interfaces.Services;
using NovillusPath.Application.Exceptions;
using NovillusPath.Domain.Entities;
using NovillusPath.Domain.Entities.Content;
using NovillusPath.Domain.Enums; // Add this using statement

namespace NovillusPath.Application.Services;

public class LessonService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : ILessonService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    public async Task<LessonDto> CreateLessonAsync(Guid sectionId, CreateLessonDto createLessonDto, CancellationToken cancellationToken)
    {
        // 1. Authorization & Parent Entity Validation:
        var section = await _unitOfWork.SectionRepository.GetSectionWithCourseAsync(sectionId, cancellationToken)
           ?? throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        if (section.Course.InstructorId != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
        {
            throw new ServiceAuthorizationException("You are not authorized to create lessons in this section.");
        }

        // 2. Map CreateLessonDto to Lesson entity:
        var lessonToCreate = _mapper.Map<Lesson>(createLessonDto);
        lessonToCreate.SectionId = sectionId;

        // 3. Handle Lesson Order:
        var lessonsInSection = (await _unitOfWork.LessonRepository.ListAsync(l => l.SectionId == sectionId, l => l.Order, true, cancellationToken)).ToList();
        if (createLessonDto.Order is null)
        {
            lessonToCreate.Order = lessonsInSection.Count > 0 ? lessonsInSection.Max(l => l.Order) + 1 : 0;
        }
        else
        {
            var specifiedOrder = createLessonDto.Order.Value;
            if (specifiedOrder < 0) throw new ServiceBadRequestException("Order cannot be negative.");

            lessonToCreate.Order = specifiedOrder;

            var lessonsToShift = lessonsInSection.Where(l => l.Order >= specifiedOrder).ToList();
            if (lessonsToShift.Count != 0)
            {
                foreach (var lesson in lessonsToShift.OrderBy(l => l.Order))
                {
                    lesson.Order++;
                    lesson.UpdatedAt = DateTime.UtcNow;

                }
            }
        }

        // 4. Process and Create ContentBlock Entities:
        if (createLessonDto.ContentBlocks != null && createLessonDto.ContentBlocks.Count > 0)
        {
            foreach (var contentBlockBaseDto in createLessonDto.ContentBlocks.OrderBy(cb => cb.Order))
            {
                ContentBlock? contentBlockEntity = null;
                contentBlockEntity = contentBlockBaseDto.Type switch
                {
                    ContentBlockType.Text => _mapper.Map<TextContent>(contentBlockBaseDto),
                    ContentBlockType.Video => _mapper.Map<VideoContent>(contentBlockBaseDto),
                    _ => throw new ServiceBadRequestException($"Unsupported content block type: {contentBlockBaseDto.Type}"),
                };
                if (contentBlockEntity != null)
                {
                    contentBlockEntity.Order = contentBlockBaseDto.Order;
                    lessonToCreate.ContentBlocks.Add(contentBlockEntity);
                }
            }
        }

        // 5. Add the new Lesson entity to the repository/DbContext.
        await _unitOfWork.LessonRepository.AddAsync(lessonToCreate, cancellationToken);

        // 6. Save changes via UnitOfWork.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Map the newly created Lesson entity (now with its ID and ContentBlocks) to LessonDto and return it.
        return _mapper.Map<LessonDto>(lessonToCreate);
    }

    public async Task<IReadOnlyList<LessonDto>> GetLessonsBySectionAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        // 1. Validate Parent Entity (Section):
        if (!await _unitOfWork.SectionRepository.ExistsAsync(s => s.Id == sectionId, cancellationToken))
        {
            throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        }

        // 2. Retrieve Lessons:
        var lessons = await _unitOfWork.LessonRepository.GetLessonsBySectionIdAsync(sectionId, true, cancellationToken);

        // 3. Map and return the Lesson entities to DTOs:
        return _mapper.Map<IReadOnlyList<LessonDto>>(lessons);

    }
    public async Task<LessonDto> GetLessonByIdAsync(Guid sectionId, Guid lessonId, CancellationToken cancellationToken)
    {
        var lesson = await _unitOfWork.LessonRepository.GetLessonWithContentBlocksAsync(lessonId, cancellationToken);
        if (lesson == null || lesson.SectionId != sectionId)
        {
            throw new ServiceNotFoundException($"Lesson with ID {lessonId} not found in section {sectionId}.");
        }
        return _mapper.Map<LessonDto>(lesson);
    }
    public async Task UpdateLessonAsync(Guid sectionId, Guid lessonId, UpdateLessonDto updateLessonDto, CancellationToken cancellationToken)
    {
        //TODO: Individual content blocks update deferred.

        // 1. Authorization & Parent Entity Validation:
        var section = await _unitOfWork.SectionRepository.GetSectionWithCourseAsync(sectionId, cancellationToken)
           ?? throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        if (section.Course.InstructorId != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
        {
            throw new ServiceAuthorizationException("You are not authorized to update lessons in this section.");
        }

        // 2. Fetch Existing Lesson
        var lessonToUpdate = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lessonToUpdate == null || lessonToUpdate.SectionId != sectionId)
        {
            throw new ServiceNotFoundException($"Lesson with ID {lessonId} not found in section {sectionId}.");
        }

        // 3. Handle Lesson Order
        if (updateLessonDto.Order.HasValue && updateLessonDto.Order.Value != lessonToUpdate.Order)
        {
            // Fetching all lessons in the section excluding the current one
            var lessonsInSection = (await _unitOfWork.LessonRepository.ListAsync(
                l => l.SectionId == sectionId && l.Id != lessonId,
                l => l.Order, true, cancellationToken)).ToList();

            var originalOrder = lessonToUpdate.Order;
            var newOrderValue = updateLessonDto.Order.Value;

            if (newOrderValue < 0)
            {
                throw new ServiceBadRequestException("Order cannot be negative.");
            }

            // Logic for shifting other lessons
            if (newOrderValue > originalOrder)
            {
                // Moving to a higher order number (e.g., from 2 to 5)
                // Lessons that were at originalOrder + 1 ... newOrderValue need to have their order decremented.
                foreach (var lesson in lessonsInSection.Where(l => l.Order > originalOrder && l.Order <= newOrderValue)
                                                    .OrderByDescending(l => l.Order)) // Process from higher to lower to avoid conflicts
                {
                    lesson.Order--;
                    lesson.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                foreach (var lesson in lessonsInSection.Where(l => l.Order >= newOrderValue && l.Order < originalOrder).OrderBy(l => l.Order))
                {
                    lesson.Order++;
                    lesson.UpdatedAt = DateTime.UtcNow;
                }
            }
            lessonToUpdate.Order = newOrderValue; // Set the new order for the lesson being updated
        }

        // 4. Map DTO to Entity
        _mapper.Map(updateLessonDto, lessonToUpdate);
        lessonToUpdate.UpdatedAt = DateTime.UtcNow;

        // 5. Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteLessonAsync(Guid sectionId, Guid lessonId, CancellationToken cancellationToken)
    {
        // 1. Authorization & Parent Entity Validation
        var section = await _unitOfWork.SectionRepository.GetSectionWithCourseAsync(sectionId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        if (section.Course.InstructorId != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
        {
            throw new ServiceAuthorizationException("You are not authorized to delete lessons in this section.");
        }

        // 2. Fetch Existing Lesson
        var lessonToDelete = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lessonToDelete == null || lessonToDelete.SectionId != sectionId)
        {
            throw new ServiceNotFoundException($"Lesson with ID {lessonId} not found in section {sectionId}.");
        }

        // 3. Re-order Subsequent Lessons
        var lessonsToReOrder = (await _unitOfWork.LessonRepository.ListAsync(l => l.SectionId == sectionId && l.Order > lessonToDelete.Order, l => l.Order, true, cancellationToken)).ToList();
        if (lessonsToReOrder.Count != 0)
        {
            foreach (var lesson in lessonsToReOrder) // Already ordered by 'Order' ascending
            {
                lesson.Order--;
                lesson.UpdatedAt = DateTime.UtcNow;
            }
        }

        // 4. Delete Lesson and save
        await _unitOfWork.LessonRepository.DeleteAsync(lessonToDelete, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
