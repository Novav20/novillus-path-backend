using NovillusPath.Application.DTOs.Review;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.Review;

public class UpdateReviewDtoValidation : BaseValidator<UpdateReviewDto>
{
    public UpdateReviewDtoValidation()
    {
        RuleForOptionalByte(r => r.Rating, 1, 5);

        RuleForOptionalString(r => r.Comment, 1000);
    }
}