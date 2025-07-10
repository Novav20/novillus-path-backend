using FluentValidation;
using NovillusPath.Application.DTOs.Review;

namespace NovillusPath.Application.Validation.Review;

public class CreateReviewDtoValidation : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidation()
    {
        // Rule for Rating (which is byte, non-nullable in CreateReviewDto)
        RuleFor(r => r.Rating)
            .InclusiveBetween((byte)1, (byte)5)
            .WithMessage("Rating must be between 1 and 5.");

        // Rule for Comment (string?, nullable)
        RuleFor(r => r.Comment)
            .MaximumLength(1000).WithMessage("{PropertyName} must not exceed 1000 characters.")
            .NotEmpty().When(r => r.Comment != null);
    }
}