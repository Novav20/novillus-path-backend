using NovillusPath.Application.DTOs.Category;

namespace NovillusPath.Application.Validation.Category;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty().WithMessage("{PropertyName} is required if provided.")
            .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.")
            .MinimumLength(3).WithMessage("{PropertyName} must be at least 3 characters long.")
            .When(dto => dto.Name != null); // Only validate if Name is part of the update payload

        RuleFor(dto => dto.Description)
            .MaximumLength(500).WithMessage("{PropertyName} must not exceed 500 characters.")
            .When(dto => dto.Description != null); // Only validate if Description is part of the update payload
    }
}