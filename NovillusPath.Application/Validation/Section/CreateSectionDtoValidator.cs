using NovillusPath.Application.DTOs.Section;

namespace NovillusPath.Application.Validation.Section;

public class CreateSectionDtoValidator : AbstractValidator<CreateSectionDto>
{
    public CreateSectionDtoValidator()
    {
        RuleFor(s => s.Title)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(150).WithMessage("{PropertyName} must not exceed 150 characters.");

        RuleFor(s => s.Order)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than or equal to 0.")
            .When(s => s.Order.HasValue);
    }
}
