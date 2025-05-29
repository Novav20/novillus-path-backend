using FluentValidation;
using NovillusPath.Application.DTOs.Category;

namespace NovillusPath.Application.Validation.Category;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.")
            .MinimumLength(3).WithMessage("{PropertyName} must be at least 3 characters long.");

        RuleFor(dto => dto.Description)
            .MaximumLength(500).WithMessage("{PropertyName} must not exceed 500 characters.")
            .When(dto => !string.IsNullOrEmpty(dto.Description)); // Only validate if description is provided
    }
}