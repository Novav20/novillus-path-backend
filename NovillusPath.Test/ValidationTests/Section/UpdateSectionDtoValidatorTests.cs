using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.Section;
using NovillusPath.Application.Validation.Section;

namespace NovillusPath.Test.ValidationTests.Section;

public class UpdateSectionDtoValidatorTests
{
    private readonly UpdateSectionDtoValidator _validator;

    public UpdateSectionDtoValidatorTests()
    {
        _validator = new UpdateSectionDtoValidator();
    }

    [Fact]
    public void Title_ShouldNotBeEmpty_WhenProvided()
    {
        var dto = new UpdateSectionDto { Title = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Title es requerido si se proporciona.");
    }

    [Fact]
    public void Title_ShouldNotExceedMaxLength_WhenProvided()
    {
        var dto = new UpdateSectionDto { Title = new string('a', 151) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Title no debe exceder los 150 caracteres.");
    }

    [Fact]
    public void Order_ShouldBeGreaterThanOrEqualToZero_WhenProvided()
    {
        var dto = new UpdateSectionDto { Order = -1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Order)
              .WithErrorMessage("Order debe ser mayor o igual a {MinValue}.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new UpdateSectionDto { Title = "Valid Title", Order = 1 };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenOnlyTitleIsProvided()
    {
        var dto = new UpdateSectionDto { Title = "Valid Title" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenOnlyOrderIsProvided()
    {
        var dto = new UpdateSectionDto { Order = 5 };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenNoPropertiesAreProvided()
    {
        var dto = new UpdateSectionDto { };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}