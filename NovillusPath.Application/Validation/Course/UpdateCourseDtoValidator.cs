using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Course;

public class UpdateCourseDtoValidator : BaseValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("{PropertyName} es requerido si se proporciona.")
            .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los 100 caracteres.")
            .When(c => c.Title != null);

        RuleForOptionalString(c => c.Description, 1000);

        RuleForOptionalDecimal(c => c.Price, 0);

        RuleFor(p => p.Status)
            .IsInEnum().WithMessage("Estado de curso inválido.")
            .When(p => p.Status.HasValue);

        RuleFor(c => c.DurationInWeeks)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} debe ser positivo.")
            .LessThanOrEqualTo(52).WithMessage("{PropertyName} debe ser menor o igual a 52.")
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

