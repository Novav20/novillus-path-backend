using SourceGuild.Application.DTOs.Section;
using SourceGuild.Application.Validation.Common;

namespace SourceGuild.Application.Validation.Section;

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
