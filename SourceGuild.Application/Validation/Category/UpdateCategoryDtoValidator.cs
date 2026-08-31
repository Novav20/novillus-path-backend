using SourceGuild.Application.DTOs.Category;
using SourceGuild.Application.Validation.Common;

namespace SourceGuild.Application.Validation.Category;

public class UpdateCategoryDtoValidator : BaseValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty().WithMessage("{PropertyName} is required if provided.")
            .MinimumLength(3).WithMessage("{PropertyName} must have at least {MinLength} characters.")
            .MaximumLength(100).WithMessage("{PropertyName} cannot exceed {MaxLength} characters.")
            .When(dto => dto.Name != null);

        RuleForOptionalString(dto => dto.Description, 500);
    }
}