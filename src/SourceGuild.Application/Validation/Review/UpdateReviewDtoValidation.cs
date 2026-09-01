using SourceGuild.Application.DTOs.Review;
using SourceGuild.Application.Validation.Common;

namespace SourceGuild.Application.Validation.Review;

public class UpdateReviewDtoValidation : BaseValidator<UpdateReviewDto>
{
    public UpdateReviewDtoValidation()
    {
        RuleForOptionalByte(r => r.Rating, 1, 5);

        RuleForOptionalString(r => r.Comment, 1000);
    }
}