using System;
using AutoMapper;
using NovillusPath.Application.Exceptions;
using NovillusPath.Application.Helpers;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Application.Interfaces.Services;
using NovillusPath.Domain.Entities;
using NovillusPath.Domain.Enums;

namespace NovillusPath.Application.Services;

public class EnrollmentService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    public async Task EnrollAsync(Guid courseId, Guid userId, CancellationToken cancellationToken) // userId is the ID of the user TO BE ENROLLED
    {
        // 1. Authorization Check: Who is the current user, and can they enroll 'userId'?
        //    The 'userId' parameter is the student being enrolled.
        //    _currentUserService represents the user MAKING the request.
        if (!AuthorizationHelper.CanPerformEnrollmentAction(_currentUserService, userId))
        {
            throw new ServiceAuthorizationException("You are not authorized to perform this enrollment action.");
        }

        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken)
            ?? throw new ServiceNotFoundException($"Course with ID {courseId} not found.");

        if (course.Status != CourseStatus.Published)
        {
            throw new ServiceBadRequestException("The course is not available for enrollment as it is not published.");
        }

        bool alreadyEnrolled = await _unitOfWork.EnrollmentRepository.ExistsAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken);
        if (alreadyEnrolled)
        {
            throw new ServiceBadRequestException("User is already enrolled in this course.");
        }

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId
        };

        await _unitOfWork.EnrollmentRepository.AddAsync(enrollment, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UnenrollAsync(Guid courseId, Guid userId, CancellationToken cancellationToken)
    {
        if (!AuthorizationHelper.CanPerformEnrollmentAction(_currentUserService, userId))
        {
            throw new ServiceAuthorizationException("You are not authorized to perform this unenrollment action.");
        }

        var enrollment = await _unitOfWork.EnrollmentRepository.GetByUserIdAndCourseIdAsync(userId, courseId, cancellationToken) ?? throw new ServiceNotFoundException("Enrollment not found.");
        await _unitOfWork.EnrollmentRepository.DeleteAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
