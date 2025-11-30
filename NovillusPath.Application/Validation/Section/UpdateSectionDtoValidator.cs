using NovillusPath.Application.DTOs.Section;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Section;

public class UpdateSectionDtoValidator : BaseValidator<UpdateSectionDto>
{
    public UpdateSectionDtoValidator()
    {
        RuleFor(s => s.Title)
            .NotEmpty().WithMessage("{PropertyName} is required if provided.")
            .MaximumLength(150).WithMessage("{PropertyName} cannot exceed 150 characters.")
            .When(s => s.Title != null);

        RuleForOptionalInteger(s => s.Order, 0);
    }
}
