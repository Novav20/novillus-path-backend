using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Course;

public class CreateCourseDtoValidator : BaseValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleForRequiredString(c => c.Title, 3, 100);

        RuleForRequiredDecimal(c => c.Price, 0);

        RuleForOptionalString(c => c.Description, 1000);

        RuleFor(c => c.DurationInWeeks)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than or equal to 0.")
            .LessThanOrEqualTo(52).WithMessage("{PropertyName} must be less than or equal to 52.")
            .Must(val => val.HasValue && val.Value % 1 == 0).WithMessage("{PropertyName} must be an integer.")
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
