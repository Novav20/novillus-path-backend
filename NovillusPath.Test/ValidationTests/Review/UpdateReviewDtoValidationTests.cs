using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.Review;
using NovillusPath.Application.Validation.Review;

namespace NovillusPath.Test.ValidationTests.Review;

public class UpdateReviewDtoValidationTests
{
    private readonly UpdateReviewDtoValidation _validator;

    public UpdateReviewDtoValidationTests()
    {
        _validator = new UpdateReviewDtoValidation();
    }

    [Fact]
    public void Rating_ShouldBeBetween1And5_WhenProvided()
    {
        var dto = new UpdateReviewDto { Rating = 0 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Rating)
              .WithErrorMessage("Rating debe estar entre {MinValue} y {MaxValue}.");

        dto = new UpdateReviewDto { Rating = 6 };
        result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Rating)
              .WithErrorMessage("Rating debe estar entre {MinValue} y {MaxValue}.");
    }

    [Fact]
    public void Comment_ShouldNotExceedMaxLength_WhenProvided()
    {
        var dto = new UpdateReviewDto { Comment = new string('a', 1001) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Comment)
              .WithErrorMessage("Comment no debe exceder los 1000 caracteres.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new UpdateReviewDto { Rating = 3, Comment = "Valid Comment" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenOnlyRatingIsProvided()
    {
        var dto = new UpdateReviewDto { Rating = 4 };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenOnlyCommentIsProvided()
    {
        var dto = new UpdateReviewDto { Comment = "Another valid comment" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenNoPropertiesAreProvided()
    {
        var dto = new UpdateReviewDto { };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}