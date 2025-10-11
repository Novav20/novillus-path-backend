using NovillusPath.Application.DTOs.Section;
using System.Linq.Expressions; // Add this using statement

namespace NovillusPath.Application.Services;


public class SectionService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : ISectionService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<IReadOnlyList<SectionDto>> GetSectionsAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        var currentUserId = _currentUserService.UserId;
        bool isAdmin = _currentUserService.IsInRole("Admin");
        bool isInstructor = course.InstructorId == currentUserId;
        if (!VisibilityHelper.CanUserViewCourse(course, _currentUserService))
        {
            throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        }
        Expression<Func<Section, bool>> sectionFilter;
        bool privileged = isAdmin || isInstructor;
        sectionFilter = privileged ? (s => s.CourseId == courseId) : (s => s.CourseId == courseId && s.Status == SectionStatus.Published);
        var sections = await _unitOfWork.SectionRepository.ListAsync(
            sectionFilter,
            s => s.Order,
            true,
            cancellationToken);
        return _mapper.Map<IReadOnlyList<SectionDto>>(sections);
    }

    public async Task<SectionDto> GetSectionByIdAsync(Guid courseId, Guid sectionId, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.SectionRepository.GetFullSectionByIdAsync(sectionId, cancellationToken);
        if (section == null || section.CourseId != courseId)
        {
            throw new ServiceNotFoundException($"Section with ID {sectionId} not found in course {courseId}.");
        }
        var currentUserId = _currentUserService.UserId;
        bool isAdmin = _currentUserService.IsInRole("Admin");
        bool isInstructor = section.Course.InstructorId == currentUserId;
        if (!VisibilityHelper.CanUserViewSection(section, _currentUserService))
        {
            throw new ServiceNotFoundException($"Section with ID {sectionId} not found in course {courseId}.");
        }
        bool isPublicOrStudentViewForLessons = !isAdmin && !isInstructor;
        if (isPublicOrStudentViewForLessons)
        {
            section.Lessons = [.. section.Lessons.Where(l => l.Status == LessonStatus.Published)];
        }
        return _mapper.Map<SectionDto>(section);
    }

    public async Task<SectionDto> CreateSectionAsync(Guid courseId, CreateSectionDto createSectionDto, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetDetailedCourseConditionallyAsync(courseId, true, false, true, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        if (!AuthorizationHelper.CanEditCourse(_currentUserService, course.InstructorId))
        {
            throw new ServiceAuthorizationException("You are not authorized to create sections in this course.");
        }
        var sectionToCreate = _mapper.Map<Section>(createSectionDto);
        sectionToCreate.CourseId = courseId;
        var sectionsInCourse = (await _unitOfWork.SectionRepository.ListAsync(s => s.CourseId == courseId, s => s.Order, true, cancellationToken)).ToList();
        if (createSectionDto.Order == null)
        {
            sectionToCreate.Order = sectionsInCourse.Count > 0 ? sectionsInCourse.Max(s => s.Order) + 1 : 0;
        }
        else
        {
            var specifiedOrder = createSectionDto.Order.Value;
            if (specifiedOrder < 0)
            {
                throw new ServiceBadRequestException("Order cannot be negative.");
            }
            sectionToCreate.Order = specifiedOrder;
            OrderManager.ShiftOrderForInsert(sectionsInCourse, specifiedOrder, s => s.Order, (s, o) => s.Order = o, s => s.UpdatedAt = DateTime.UtcNow);
        }
        await _unitOfWork.SectionRepository.AddAsync(sectionToCreate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<SectionDto>(sectionToCreate);
    }

    public async Task UpdateSectionAsync(Guid courseId, Guid sectionId, UpdateSectionDto updateSectionDto, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        if (!AuthorizationHelper.CanEditCourse(_currentUserService, course.InstructorId))
        {
            throw new ServiceAuthorizationException("You are not authorized to update sections in this course.");
        }
        var sectionToUpdate = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId, cancellationToken);
        if (sectionToUpdate == null || sectionToUpdate.CourseId != courseId)
        {
            throw new ServiceNotFoundException($"Section with ID {sectionId} not found in course {courseId} for update.");
        }
        var originalOrder = sectionToUpdate.Order;
        var newOrder = updateSectionDto.Order;
        _mapper.Map(updateSectionDto, sectionToUpdate);
        sectionToUpdate.UpdatedAt = DateTime.UtcNow;
        if (newOrder.HasValue && newOrder != originalOrder)
        {
            if (newOrder.Value < 0)
            {
                throw new ServiceBadRequestException("Order cannot be negative.");
            }
            var sectionsInCourse = (await _unitOfWork.SectionRepository.ListAsync(s => s.CourseId == courseId && s.Id != sectionId, q => q.Order, true, cancellationToken)).ToList();
            OrderManager.ShiftOrderForUpdate(sectionsInCourse, originalOrder, newOrder.Value, s => s.Order, (s, o) => s.Order = o, s => s.UpdatedAt = DateTime.UtcNow);
            sectionToUpdate.Order = newOrder.Value;
        }
        else if (!newOrder.HasValue)
        {
            sectionToUpdate.Order = originalOrder;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteSectionAsync(Guid courseId, Guid sectionId, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        if (!AuthorizationHelper.CanEditCourse(_currentUserService, course.InstructorId))
        {
            throw new ServiceAuthorizationException("You are not authorized to delete sections in this course.");
        }
        var sectionToDelete = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId, cancellationToken);
        if (sectionToDelete == null || sectionToDelete.CourseId != courseId)
        {
            throw new ServiceNotFoundException($"Section with ID {sectionId} not found in course {courseId} for deletion.");
        }
        var deletedOrder = sectionToDelete.Order;
        await _unitOfWork.SectionRepository.DeleteAsync(sectionToDelete, cancellationToken);
        var sectionsToReOrder = (await _unitOfWork.SectionRepository.ListAsync(s => s.CourseId == courseId && s.Order > deletedOrder, s => s.Order, true, cancellationToken)).ToList();
        OrderManager.ShiftOrderForDelete(sectionsToReOrder, deletedOrder, s => s.Order, (s, o) => s.Order = o, s => s.UpdatedAt = DateTime.UtcNow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSectionStatusAsync(Guid courseId, Guid sectionId, UpdateSectionStatusDto updateStatusDto, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        if (!AuthorizationHelper.CanEditCourse(_currentUserService, course.InstructorId))
            throw new ServiceAuthorizationException("You are not authorized to update section status in this course.");
        var section = await _unitOfWork.SectionRepository.GetSectionWithLessonsAsync(sectionId, cancellationToken);
        if (section == null || section.CourseId != courseId)
            throw new ServiceNotFoundException($"Section with ID {sectionId} not found in course {courseId}.");
        if (!Enum.TryParse<SectionStatus>(updateStatusDto.Status, true, out var newStatus))
            throw new ServiceBadRequestException($"Invalid section status: '{updateStatusDto.Status}'. Valid values are {string.Join(", ", Enum.GetNames<SectionStatus>())}.");
        if (newStatus == SectionStatus.Published && course.Status != CourseStatus.Published)
            throw new ServiceBadRequestException("Cannot publish section when its parent course is not published.");
        if ((newStatus == SectionStatus.Draft || newStatus == SectionStatus.Archived) && section.Status == SectionStatus.Published)
        {
            LessonStatus targetLessonStatus = (newStatus == SectionStatus.Draft) ? LessonStatus.Draft : LessonStatus.Archived;
            foreach (var lesson in section.Lessons.Where(l => l.Status == LessonStatus.Published))
            {
                lesson.Status = targetLessonStatus;
                lesson.UpdatedAt = DateTime.UtcNow;
            }
        }
        section.Status = newStatus;
        section.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
