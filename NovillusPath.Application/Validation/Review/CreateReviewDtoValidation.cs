using NovillusPath.Application.DTOs.Review;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Review;

public class CreateReviewDtoValidation : BaseValidator<CreateReviewDto>
{
    public CreateReviewDtoValidation()
    {
        RuleForRequiredByte(r => r.Rating, 1, 5);

        RuleFor(r => r.Comment)
            .MaximumLength(1000).WithMessage("{PropertyName} no debe exceder los 1000 caracteres.")
            .NotEmpty().When(r => r.Comment != null);
    }
}