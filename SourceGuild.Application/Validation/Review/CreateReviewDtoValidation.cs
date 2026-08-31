using SourceGuild.Application.DTOs.Review;
using SourceGuild.Application.Validation.Common;

namespace SourceGuild.Application.Validation.Review;

public class CreateReviewDtoValidation : BaseValidator<CreateReviewDto>
{
    public CreateReviewDtoValidation()
    {
        RuleForRequiredByte(r => r.Rating, 1, 5);

        RuleFor(r => r.Comment)
            .MaximumLength(1000).WithMessage("{PropertyName} cannot exceed 1000 characters.")
            .NotEmpty().When(r => r.Comment != null);
    }
}