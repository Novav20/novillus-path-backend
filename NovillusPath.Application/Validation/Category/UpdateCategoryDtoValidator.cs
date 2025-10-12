using NovillusPath.Application.DTOs.Category;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Category;

public class UpdateCategoryDtoValidator : BaseValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty().WithMessage("{PropertyName} es requerido si se proporciona.")
            .MinimumLength(3).WithMessage("{PropertyName} debe tener al menos {MinLength} caracteres.")
            .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres.")
            .When(dto => dto.Name != null);

        RuleForOptionalString(dto => dto.Description, 500);
    }
}