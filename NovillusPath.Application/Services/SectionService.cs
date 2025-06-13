using AutoMapper;
using NovillusPath.Application.DTOs.Section;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Application.Interfaces.Services;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Services;

// Define a custom exception for authorization errors within the service
public class ServiceAuthorizationException(string message) : Exception(message)
{
}

// Define a custom exception for not found errors within the service
public class ServiceNotFoundException(string message) : Exception(message)
{
}

// Define a custom exception for bad request errors within the service
public class ServiceBadRequestException(string message) : Exception(message)
{
}


public class SectionService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : ISectionService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<IReadOnlyList<SectionDto>> GetSectionsAsync(Guid courseId, CancellationToken cancellationToken)
    {
        bool courseExists = await _unitOfWork.CourseRepository.ExistsAsync(c => c.Id == courseId, cancellationToken);
        if (!courseExists)
        {
            throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        }

        var sections = await _unitOfWork.SectionRepository.ListAsync(s => s.CourseId == courseId, s => s.Order, true, cancellationToken);
        return _mapper.Map<IReadOnlyList<SectionDto>>(sections);
    }

    public async Task<SectionDto> GetSectionByIdAsync(Guid courseId, Guid sectionId, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId, cancellationToken);

        if (section == null || section.CourseId != courseId)
        {
            throw new ServiceNotFoundException($"Section with ID {sectionId} not found in course {courseId}.");
        }
        return _mapper.Map<SectionDto>(section);
    }

    public async Task<SectionDto> CreateSectionAsync(Guid courseId, CreateSectionDto createSectionDto, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetCourseWithDetailsAsync(courseId, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        if (course.InstructorId != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
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

            // Shift sections if the specified order already exists or to fill a gap appropriately
            var sectionsToShift = sectionsInCourse.Where(s => s.Order >= specifiedOrder).ToList();
            if (sectionsToShift.Count != 0)
            {
                foreach (var sec in sectionsToShift.OrderBy(s => s.Order)) // Process in order to avoid conflicts if shifting by more than 1
                {
                    sec.Order++;
                    sec.UpdatedAt = DateTime.UtcNow;
                    // No need to call UpdateAsync here, SaveChangesAsync will handle it
                }
            }
        }

        await _unitOfWork.SectionRepository.AddAsync(sectionToCreate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken); // This will save the new section and updated orders of shifted sections

        return _mapper.Map<SectionDto>(sectionToCreate);
    }

    public async Task UpdateSectionAsync(Guid courseId, Guid sectionId, UpdateSectionDto updateSectionDto, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        if (course.InstructorId != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
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

            // Scenario 1: Moving a section to a lower order (e.g., 5th to 2nd)
            // Sections from newOrder up to originalOrder-1 need to be incremented
            if (newOrder < originalOrder)
            {
                foreach (var s in sectionsInCourse.Where(s => s.Order >= newOrder && s.Order < originalOrder).OrderBy(s => s.Order))
                {
                    s.Order++;
                    s.UpdatedAt = DateTime.UtcNow;
                }
            }
            // Scenario 2: Moving a section to a higher order (e.g., 2nd to 5th)
            // Sections from originalOrder+1 up to newOrder need to be decremented
            else
            {
                foreach (var s in sectionsInCourse.Where(s => s.Order > originalOrder && s.Order <= newOrder).OrderByDescending(s => s.Order))
                {
                    s.Order--;
                    s.UpdatedAt = DateTime.UtcNow;
                }
            }
            sectionToUpdate.Order = newOrder.Value;
        }
        else if (!newOrder.HasValue) // If order is explicitly set to null in DTO, treat it as no change to order.
        {
            // If UpdateSectionDto.Order is nullable and not provided, retain original order.
            // If it's not nullable, this branch is not needed as mapping would handle it or validation.
            // For this implementation, we assume Order in UpdateSectionDto is nullable.
            // If it was intended to remove ordering, that's a different feature.
            sectionToUpdate.Order = originalOrder; // Ensure it's not accidentally changed by mapper if DTO has null
        }


        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteSectionAsync(Guid courseId, Guid sectionId, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken) ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");
        if (course.InstructorId != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
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

        // Re-order subsequent sections
        var sectionsToReOrder = (await _unitOfWork.SectionRepository.ListAsync(s => s.CourseId == courseId && s.Order > deletedOrder, s => s.Order, true, cancellationToken)).ToList();
        if (sectionsToReOrder.Any())
        {
            foreach (var sec in sectionsToReOrder) // Already ordered by 'Order' ascending
            {
                sec.Order--;
                sec.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
