using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Validation.Course;

namespace NovillusPath.Test.ValidationTests.Course;

public class CreateCourseDtoValidatorTests
{
    private readonly CreateCourseDtoValidator _validator;

    public CreateCourseDtoValidatorTests()
    {
        _validator = new CreateCourseDtoValidator();
    }

    [Fact]
    public void Title_ShouldNotBeEmpty()
    {
        var dto = new CreateCourseDto { Title = "", Price = 10.0m };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Title es requerido.");
    }

    [Fact]
    public void Title_ShouldNotExceedMaxLength()
    {
        var dto = new CreateCourseDto { Title = new string('a', 101), Price = 10.0m };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Title no debe exceder los 100 caracteres.");
    }

    [Fact]
    public void Price_ShouldBeGreaterThanOrEqualToZero()
    {
        var dto = new CreateCourseDto { Title = "Valid Title", Price = -1.0m };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Price)
              .WithErrorMessage("Price debe ser mayor o igual a {MinValue}.");
    }

    [Fact]
    public void Description_ShouldNotExceedMaxLength()
    {
        var dto = new CreateCourseDto { Title = "Valid Title", Price = 10.0m, Description = new string('a', 1001) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description no debe exceder los 1000 caracteres.");
    }

    [Fact]
    public void DurationInWeeks_ShouldBeGreaterThanOrEqualToZero()
    {
        var dto = new CreateCourseDto { Title = "Valid Title", Price = 10.0m, DurationInWeeks = -1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DurationInWeeks)
              .WithErrorMessage("Duration In Weeks debe ser mayor o igual a 0.");
    }

    [Fact]
    public void DurationInWeeks_ShouldBeLessThanOrEqualTo52()
    {
        var dto = new CreateCourseDto { Title = "Valid Title", Price = 10.0m, DurationInWeeks = 53 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DurationInWeeks)
              .WithErrorMessage("Duration In Weeks debe ser menor o igual a 52.");
    }

    [Fact]
    public void DurationInWeeks_ShouldBeAnInteger()
    {
        // This rule is for decimal values, but DurationInWeeks is int?, so this test might not be directly applicable
        // The current rule is `Must(val => val.HasValue && val.Value % 1 == 0)` which is always true for int?
        // We'll test that a valid int passes.
        var dto = new CreateCourseDto { Title = "Valid Title", Price = 10.0m, DurationInWeeks = 10 };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.DurationInWeeks);
    }

    [Fact]
    public void ImageUrl_ShouldNotExceedMaxLength()
    {
        var dto = new CreateCourseDto { Title = "Valid Title", Price = 10.0m, ImageUrl = new string('a', 1001) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
              .WithErrorMessage("Image Url no debe exceder los 1000 caracteres.");
    }

    [Fact]
    public void StartDate_ShouldBeInTheFuture()
    {
        var dto = new CreateCourseDto { Title = "Valid Title", Price = 10.0m, StartDate = DateTime.UtcNow.AddDays(-1) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
              .WithErrorMessage("Start Date debe ser una fecha futura.");
    }

    [Fact]
    public void CategoryIds_ShouldNotContainEmptyGuids()
    {
        var dto = new CreateCourseDto { Title = "Valid Title", Price = 10.0m, CategoryIds = new List<Guid> { Guid.NewGuid(), Guid.Empty } };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CategoryIds)
              .WithErrorMessage("Los IDs de categoría en la lista no pueden ser GUIDs vacíos.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new CreateCourseDto
        {
            Title = "Valid Title",
            Price = 10.0m,
            Description = "Valid Description",
            DurationInWeeks = 12,
            ImageUrl = "http://example.com/image.jpg",
            StartDate = DateTime.UtcNow.AddDays(1),
            CategoryIds = new List<Guid> { Guid.NewGuid() }
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenOptionalPropertiesAreNull()
    {
        var dto = new CreateCourseDto
        {
            Title = "Valid Title",
            Price = 10.0m,
            Description = null,
            DurationInWeeks = null,
            ImageUrl = null,
            StartDate = null,
            CategoryIds = null
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}