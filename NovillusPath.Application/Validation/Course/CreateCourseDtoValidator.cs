using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Course;

public class CreateCourseDtoValidator : BaseValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleForRequiredString(c => c.Title, 3, 100);

        RuleForRequiredDecimal(c => c.Price, 0);

        RuleForOptionalString(c => c.Description, 1000);

        RuleFor(c => c.DurationInWeeks)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} debe ser mayor o igual a 0.")
            .LessThanOrEqualTo(52).WithMessage("{PropertyName} debe ser menor o igual a 52.")
            .Must(val => val.HasValue && val.Value % 1 == 0).WithMessage("{PropertyName} debe ser un número entero.")
            .When(c => c.DurationInWeeks.HasValue);

        RuleFor(c => c.ImageUrl)
            .MaximumLength(1000).WithMessage("{PropertyName} no debe exceder los 1000 caracteres.")
            .When(c => !string.IsNullOrEmpty(c.ImageUrl));

        RuleForOptionalFutureDate(c => c.StartDate);

        RuleFor(dto => dto.CategoryIds)
            .Must(list => list == null || list.All(id => id != Guid.Empty)).WithMessage("Los IDs de categoría en la lista no pueden ser GUIDs vacíos.")
            .When(dto => dto.CategoryIds != null && dto.CategoryIds.Count != 0);
    }
}
