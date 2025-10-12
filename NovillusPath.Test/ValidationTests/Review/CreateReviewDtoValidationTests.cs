using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.Review;
using NovillusPath.Application.Validation.Review;

namespace NovillusPath.Test.ValidationTests.Review;

public class CreateReviewDtoValidationTests
{
    private readonly CreateReviewDtoValidation _validator;

    public CreateReviewDtoValidationTests()
    {
        _validator = new CreateReviewDtoValidation();
    }

    [Fact]
    public void Rating_ShouldBeBetween1And5()
    {
        var dto = new CreateReviewDto { Rating = 0, Comment = "Valid Comment" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Rating)
              .WithErrorMessage("Rating debe estar entre {MinValue} y {MaxValue}.");

        dto = new CreateReviewDto { Rating = 6, Comment = "Valid Comment" };
        result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Rating)
              .WithErrorMessage("Rating debe estar entre {MinValue} y {MaxValue}.");
    }

    [Fact]
    public void Comment_ShouldNotExceedMaxLength()
    {
        var dto = new CreateReviewDto { Rating = 3, Comment = new string('a', 1001) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Comment)
              .WithErrorMessage("Comment no debe exceder los 1000 caracteres.");
    }

    [Fact]
    public void Comment_ShouldNotBeEmpty_WhenProvided()
    {
        var dto = new CreateReviewDto { Rating = 3, Comment = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Comment)
              .WithErrorMessage("'Comment' must not be empty.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new CreateReviewDto { Rating = 3, Comment = "Valid Comment" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenCommentIsNull()
    {
        var dto = new CreateReviewDto { Rating = 3, Comment = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}