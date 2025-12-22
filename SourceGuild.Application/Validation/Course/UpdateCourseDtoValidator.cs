using SourceGuild.Application.DTOs.Course;
using SourceGuild.Application.Validation.Common;

namespace SourceGuild.Application.Validation.Course;

public class UpdateCourseDtoValidator : BaseValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("{PropertyName} is required if provided.")
            .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.")
            .When(c => c.Title != null);

        RuleForOptionalString(c => c.Description, 1000);

        RuleForOptionalDecimal(c => c.Price, 0);

        RuleFor(p => p.Status)
            .IsInEnum().WithMessage("Invalid course status.")
            .When(p => p.Status.HasValue);

        RuleFor(c => c.DurationInWeeks)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be positive.")
            .LessThanOrEqualTo(52).WithMessage("{PropertyName} must be less than or equal to 52.")
            .When(c => c.DurationInWeeks.HasValue);

        RuleFor(c => c.ImageUrl)
            .MaximumLength(1000).WithMessage("{PropertyName} cannot exceed 1000 characters.")
            .When(c => !string.IsNullOrEmpty(c.ImageUrl));

        RuleForOptionalFutureDate(c => c.StartDate);

        RuleFor(dto => dto.CategoryIds)
        .Must(list => list == null || list.All(id => id != Guid.Empty)).WithMessage("The IDs in the list cannot be empty GUIDs.")
        .When(dto => dto.CategoryIds != null && dto.CategoryIds.Count != 0); 
    }
}

