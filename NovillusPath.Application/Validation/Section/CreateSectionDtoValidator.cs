using NovillusPath.Application.DTOs.Section;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Section;

public class CreateSectionDtoValidator : BaseValidator<CreateSectionDto>
{
    public CreateSectionDtoValidator()
    {
        RuleForRequiredString(s => s.Title, 3, 150);

        RuleForOptionalInteger(s => s.Order, 0);
    }
}
