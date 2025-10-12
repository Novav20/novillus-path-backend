using NovillusPath.Application.DTOs.Category;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Category;

public class CreateCategoryDtoValidator : BaseValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleForRequiredString(dto => dto.Name, 3, 100);

        RuleForOptionalString(dto => dto.Description, 500);
    }
}