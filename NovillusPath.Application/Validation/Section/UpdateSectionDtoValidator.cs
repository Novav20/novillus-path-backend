using NovillusPath.Application.DTOs.Section;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Section;

public class UpdateSectionDtoValidator : BaseValidator<UpdateSectionDto>
{
    public UpdateSectionDtoValidator()
    {
        RuleFor(s => s.Title)
            .NotEmpty().WithMessage("{PropertyName} es requerido si se proporciona.")
            .MaximumLength(150).WithMessage("{PropertyName} no debe exceder los 150 caracteres.")
            .When(s => s.Title != null);

        RuleForOptionalInteger(s => s.Order, 0);
    }
}
