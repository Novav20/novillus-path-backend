using FluentValidation;
using NovillusPath.Application.DTOs.Course;

namespace NovillusPath.Application.Validation.Course;

public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");

        RuleFor(c => c.Price)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than or equal to 0.");

        RuleFor(c => c.InstructorId)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotEqual(Guid.Empty).WithMessage("{PropertyName} must not be empty.");

        RuleFor(c => c.Description)
            .MaximumLength(1000).WithMessage("{PropertyName} must not exceed 1000 characters.")
            .When(c => !string.IsNullOrEmpty(c.Description));

        RuleFor(c => c.DurationInWeeks)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than or equal to 0.")
            .LessThanOrEqualTo(52).WithMessage("{PropertyName} must be less than or equal to 52.")
            .Must(val => val.HasValue && val.Value % 1 == 0).WithMessage("{PropertyName} must be an integer.")
            .When(c => c.DurationInWeeks.HasValue);

        RuleFor(c => c.ImageUrl)
            .MaximumLength(1000).WithMessage("{PropertyName} must not exceed 1000 characters.")
            .When(c => !string.IsNullOrEmpty(c.ImageUrl));

        RuleFor(c => c.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("{PropertyName} must be in the future.")
            .When(c => c.StartDate.HasValue);
    }
}
