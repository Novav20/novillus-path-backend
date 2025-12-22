using SourceGuild.Application.DTOs.Lesson;
using SourceGuild.Domain.Entities.Content;
using System.Linq.Expressions;

namespace SourceGuild.Application.Services;

public class LessonService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : ILessonService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    public async Task<LessonDto> CreateLessonAsync(Guid sectionId, CreateLessonDto createLessonDto, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.SectionRepository.GetSectionWithCourseAsync(sectionId, cancellationToken)
           ?? throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        if (!AuthorizationHelper.CanEditSection(_currentUserService, section.Course.InstructorId))
        {
            throw new ServiceAuthorizationException("You are not authorized to create lessons in this section.");
        }

        var lessonToCreate = _mapper.Map<Lesson>(createLessonDto);
        lessonToCreate.SectionId = sectionId;

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
            OrderManager.ShiftOrderForInsert(lessonsInSection, specifiedOrder, l => l.Order, (l, o) => l.Order = o, l => l.UpdatedAt = DateTime.UtcNow);
        }

        if (createLessonDto.ContentBlocks != null && createLessonDto.ContentBlocks.Count > 0)
        {
            foreach (var contentBlockBaseDto in createLessonDto.ContentBlocks.OrderBy(cb => cb.Order))
            {
                ContentBlock? contentBlockEntity = contentBlockBaseDto.Type switch
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

        await _unitOfWork.LessonRepository.AddAsync(lessonToCreate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<LessonDto>(lessonToCreate);
    }

    public async Task<IReadOnlyList<LessonDto>> GetLessonsBySectionAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.SectionRepository.GetFullSectionByIdAsync(sectionId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        var currentUserId = _currentUserService.UserId;
        bool isAdmin = _currentUserService.IsInRole("Admin");
        bool isInstructor = section.Course.InstructorId == currentUserId;
        if (!VisibilityHelper.CanUserViewSection(section, _currentUserService))
        {
            throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        }
        Expression<Func<Lesson, bool>> lessonFilter;
        bool privileged = isAdmin || isInstructor;
        lessonFilter = privileged ? (l => l.SectionId == sectionId) : (l => l.SectionId == sectionId && l.Status == LessonStatus.Published);
        var lessons = await _unitOfWork.LessonRepository.GetFilteredLessonsAsync(
            lessonFilter,
            true,
            cancellationToken);
        return _mapper.Map<IReadOnlyList<LessonDto>>(lessons);
    }

    public async Task<LessonDto> GetLessonByIdAsync(Guid sectionId, Guid lessonId, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.SectionRepository.GetFullSectionByIdAsync(sectionId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        var currentUserId = _currentUserService.UserId;
        bool isAdmin = _currentUserService.IsInRole("Admin");
        bool isInstructor = section.Course.InstructorId == currentUserId;
        if (!VisibilityHelper.CanUserViewSection(section, _currentUserService))
        {
            throw new ServiceNotFoundException($"Lesson with ID {lessonId} not found in section {sectionId}.");
        }
        var lesson = await _unitOfWork.LessonRepository.GetLessonWithContentBlocksAsync(lessonId, cancellationToken);
        if (lesson == null || lesson.SectionId != sectionId)
        {
            throw new ServiceNotFoundException($"Lesson with ID {lessonId} not found in section {sectionId}.");
        }
        if (!VisibilityHelper.CanUserViewLesson(lesson, section, section.Course, _currentUserService))
        {
            throw new ServiceNotFoundException($"Lesson with ID {lessonId} not found in section {sectionId}.");
        }
        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task UpdateLessonAsync(Guid sectionId, Guid lessonId, UpdateLessonDto updateLessonDto, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.SectionRepository.GetSectionWithCourseAsync(sectionId, cancellationToken)
           ?? throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        if (!AuthorizationHelper.CanEditSection(_currentUserService, section.Course.InstructorId))
        {
            throw new ServiceAuthorizationException("You are not authorized to update lessons in this section.");
        }
        var lessonToUpdate = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lessonToUpdate == null || lessonToUpdate.SectionId != sectionId)
        {
            throw new ServiceNotFoundException($"Lesson with ID {lessonId} not found in section {sectionId}.");
        }
        if (updateLessonDto.Order.HasValue && updateLessonDto.Order.Value != lessonToUpdate.Order)
        {
            var lessonsInSection = (await _unitOfWork.LessonRepository.ListAsync(
                l => l.SectionId == sectionId && l.Id != lessonId,
                l => l.Order, true, cancellationToken)).ToList();
            var originalOrder = lessonToUpdate.Order;
            var newOrderValue = updateLessonDto.Order.Value;
            if (newOrderValue < 0)
            {
                throw new ServiceBadRequestException("Order cannot be negative.");
            }
            OrderManager.ShiftOrderForUpdate(lessonsInSection, originalOrder, newOrderValue, l => l.Order, (l, o) => l.Order = o, l => l.UpdatedAt = DateTime.UtcNow);
            lessonToUpdate.Order = newOrderValue;
        }
        _mapper.Map(updateLessonDto, lessonToUpdate);
        lessonToUpdate.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteLessonAsync(Guid sectionId, Guid lessonId, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.SectionRepository.GetSectionWithCourseAsync(sectionId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        if (!AuthorizationHelper.CanEditSection(_currentUserService, section.Course.InstructorId))
        {
            throw new ServiceAuthorizationException("You are not authorized to delete lessons in this section.");
        }
        var lessonToDelete = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lessonToDelete == null || lessonToDelete.SectionId != sectionId)
        {
            throw new ServiceNotFoundException($"Lesson with ID {lessonId} not found in section {sectionId}.");
        }
        var lessonsToReOrder = (await _unitOfWork.LessonRepository.ListAsync(l => l.SectionId == sectionId && l.Order > lessonToDelete.Order, l => l.Order, true, cancellationToken)).ToList();
        OrderManager.ShiftOrderForDelete(lessonsToReOrder, lessonToDelete.Order, l => l.Order, (l, o) => l.Order = o, l => l.UpdatedAt = DateTime.UtcNow);
        await _unitOfWork.LessonRepository.DeleteAsync(lessonToDelete, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateLessonStatusAsync(Guid sectionId, Guid lessonId, UpdateLessonStatusDto updateStatusDto, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.SectionRepository.GetSectionWithCourseAsync(sectionId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Section with ID {sectionId} not found.");
        var course = section.Course;
        if (!AuthorizationHelper.CanEditSection(_currentUserService, course.InstructorId))
        {
            throw new ServiceAuthorizationException("You are not authorized to update lesson status in this section.");
        }
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lesson == null || lesson.SectionId != sectionId)
        {
            throw new ServiceNotFoundException($"Lesson with ID {lessonId} not found in section {sectionId}.");
        }
        if (!Enum.TryParse<LessonStatus>(updateStatusDto.Status, out var newStatus))
        {
            throw new ServiceBadRequestException($"Invalid lesson status: {updateStatusDto.Status}");
        }
        if (newStatus == LessonStatus.Published &&
            (section.Status != SectionStatus.Published || course.Status != CourseStatus.Published))
        {
            throw new ServiceBadRequestException("Cannot publish lesson when parent section or course is not published.");
        }
        lesson.Status = newStatus;
        lesson.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
