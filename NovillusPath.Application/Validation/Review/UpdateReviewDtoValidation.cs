using NovillusPath.Application.DTOs.Review;

namespace NovillusPath.Application.Validation.Review;

public class UpdateReviewDtoValidation : AbstractValidator<UpdateReviewDto>
{
    public UpdateReviewDtoValidation()
    {
        // Rule for Rating (byte?, nullable in UpdateReviewDto)
        RuleFor(r => r.Rating)
            .InclusiveBetween((byte)1, (byte)5) 
            .WithMessage("Rating must be between 1 and 5.")
            .When(r => r.Rating.HasValue);

        // Rule for Comment (string?, nullable)
        RuleFor(r => r.Comment)
            .MaximumLength(1000).WithMessage("{PropertyName} must not exceed 1000 characters.");
    }
}