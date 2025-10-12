using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Validation.Course;
using NovillusPath.Domain.Enums;

namespace NovillusPath.Test.ValidationTests.Course;

public class UpdateCourseDtoValidatorTests
{
    private readonly UpdateCourseDtoValidator _validator;

    public UpdateCourseDtoValidatorTests()
    {
        _validator = new UpdateCourseDtoValidator();
    }

    [Fact]
    public void Title_ShouldNotBeEmpty_WhenProvided()
    {
        var dto = new UpdateCourseDto { Title = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Title es requerido si se proporciona.");
    }

    [Fact]
    public void Title_ShouldNotExceedMaxLength_WhenProvided()
    {
        var dto = new UpdateCourseDto { Title = new string('a', 101) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Title no debe exceder los 100 caracteres.");
    }

    [Fact]
    public void Description_ShouldNotExceedMaxLength_WhenProvided()
    {
        var dto = new UpdateCourseDto { Description = new string('a', 1001) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description no debe exceder los 1000 caracteres.");
    }

    [Fact]
    public void Price_ShouldBeGreaterThanOrEqualToZero_WhenProvided()
    {
        var dto = new UpdateCourseDto { Price = -1.0m };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Price)
              .WithErrorMessage("Price debe ser mayor o igual a {MinValue}.");
    }

    [Fact]
    public void Status_ShouldBeValidEnum_WhenProvided()
    {
        var dto = new UpdateCourseDto { Status = (CourseStatus)99 }; // Invalid enum value
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Status)
              .WithErrorMessage("Estado de curso inválido.");
    }

    [Fact]
    public void DurationInWeeks_ShouldBeGreaterThanOrEqualToZero_WhenProvided()
    {
        var dto = new UpdateCourseDto { DurationInWeeks = -1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DurationInWeeks)
              .WithErrorMessage("Duration In Weeks debe ser positivo.");
    }

    [Fact]
    public void DurationInWeeks_ShouldBeLessThanOrEqualTo52_WhenProvided()
    {
        var dto = new UpdateCourseDto { DurationInWeeks = 53 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.DurationInWeeks)
              .WithErrorMessage("Duration In Weeks debe ser menor o igual a 52.");
    }

    [Fact]
    public void ImageUrl_ShouldNotExceedMaxLength_WhenProvided()
    {
        var dto = new UpdateCourseDto { ImageUrl = new string('a', 1001) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
              .WithErrorMessage("Image Url no debe exceder los 1000 caracteres.");
    }

    [Fact]
    public void StartDate_ShouldBeInTheFuture_WhenProvided()
    {
        var dto = new UpdateCourseDto { StartDate = DateTime.UtcNow.AddDays(-1) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
              .WithErrorMessage("Start Date debe ser una fecha futura.");
    }

    [Fact]
    public void CategoryIds_ShouldNotContainEmptyGuids_WhenProvided()
    {
        var dto = new UpdateCourseDto { CategoryIds = new List<Guid> { Guid.NewGuid(), Guid.Empty } };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CategoryIds)
              .WithErrorMessage("Los IDs de categoría en la lista no pueden ser GUIDs vacíos.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new UpdateCourseDto
        {
            Title = "Valid Title",
            Description = "Valid Description",
            Price = 10.0m,
            Status = CourseStatus.Published,
            DurationInWeeks = 12,
            ImageUrl = "http://example.com/image.jpg",
            StartDate = DateTime.UtcNow.AddDays(1),
            CategoryIds = new List<Guid> { Guid.NewGuid() }
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenOnlyOptionalPropertiesAreProvided()
    {
        var dto = new UpdateCourseDto
        {
            Title = "New Title",
            Price = 20.0m
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenNoPropertiesAreProvided()
    {
        var dto = new UpdateCourseDto { };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}