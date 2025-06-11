using FluentValidation;
using NovillusPath.Application.DTOs.Section;

namespace NovillusPath.Application.Validation.Section;

public class UpdateSectionDtoValidator : AbstractValidator<UpdateSectionDto>
{
    public UpdateSectionDtoValidator()
    {
        RuleFor(s => s.Title)
            .NotEmpty().WithMessage("{PropertyName} is required if provided.")
            .MaximumLength(150).WithMessage("{PropertyName} must not exceed 150 characters.")
            .When(s => s.Title != null);

        RuleFor(s => s.Order)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be 0 or greater.")
            .When(s => s.Order.HasValue);
    }
}
