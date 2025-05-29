using FluentValidation;
using NovillusPath.Application.DTOs.Course;

namespace NovillusPath.Application.Validation.Course;

public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("{PropertyName} is required if provided.")
            .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.")
            .When(c => c.Title != null);

        RuleFor(c => c.Description)
                .MaximumLength(1000).WithMessage("{PropertyName} must not exceed 1000 characters.")
                .When(c => c.Description != null);

        RuleFor(c => c.Price)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than or equal to 0.")
            .When(c => c.Price.HasValue);

        RuleFor(p => p.Status)
            .IsInEnum().WithMessage("Invalid {PropertyName}.")
            .When(p => p.Status.HasValue);

        RuleFor(c => c.DurationInWeeks)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater positive.")
            .LessThanOrEqualTo(52).WithMessage("{PropertyName} must be less than or equal to 52.")
            .When(c => c.DurationInWeeks.HasValue);

        RuleFor(c => c.ImageUrl)
            .MaximumLength(1000).WithMessage("{PropertyName} must not exceed 1000 characters.")
            .When(c => !string.IsNullOrEmpty(c.ImageUrl));

        RuleFor(c => c.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("{PropertyName} must be in the future.")
            .When(c => c.StartDate.HasValue);

        RuleForEach(dto => dto.CategoryIds)
        .NotEmpty().WithMessage("Category ID in the list cannot be empty.")
        .NotEqual(Guid.Empty).WithMessage("Category ID in the list must not be an empty GUID.")
        .When(dto => dto.CategoryIds != null && dto.CategoryIds.Count != 0); 
    }
}

