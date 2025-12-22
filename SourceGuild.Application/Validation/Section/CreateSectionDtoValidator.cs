using SourceGuild.Application.DTOs.Section;
using SourceGuild.Application.Validation.Common;

namespace SourceGuild.Application.Validation.Section;

public class CreateSectionDtoValidator : BaseValidator<CreateSectionDto>
{
    public CreateSectionDtoValidator()
    {
        RuleForRequiredString(s => s.Title, 3, 150);

        RuleForOptionalInteger(s => s.Order, 0);
    }
}
