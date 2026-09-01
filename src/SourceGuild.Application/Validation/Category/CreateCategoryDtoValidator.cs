using SourceGuild.Application.DTOs.Category;
using SourceGuild.Application.Validation.Common;

namespace SourceGuild.Application.Validation.Category;

public class CreateCategoryDtoValidator : BaseValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleForRequiredString(dto => dto.Name, 3, 100);

        RuleForOptionalString(dto => dto.Description, 500);
    }
}