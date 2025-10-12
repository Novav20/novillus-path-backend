using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.Section;
using NovillusPath.Application.Validation.Section;

namespace NovillusPath.Test.ValidationTests.Section;

public class CreateSectionDtoValidatorTests
{
    private readonly CreateSectionDtoValidator _validator;

    public CreateSectionDtoValidatorTests()
    {
        _validator = new CreateSectionDtoValidator();
    }

    [Fact]
    public void Title_ShouldNotBeEmpty()
    {
        var dto = new CreateSectionDto { Title = "", Order = 1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Title es requerido.");
    }

    [Fact]
    public void Title_ShouldNotExceedMaxLength()
    {
        var dto = new CreateSectionDto { Title = new string('a', 151), Order = 1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage("Title no debe exceder los 150 caracteres.");
    }

    [Fact]
    public void Order_ShouldBeGreaterThanOrEqualToZero_WhenProvided()
    {
        var dto = new CreateSectionDto { Title = "Valid Title", Order = -1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Order)
              .WithErrorMessage("Order debe ser mayor o igual a {MinValue}.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new CreateSectionDto { Title = "Valid Title", Order = 1 };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenOrderIsNull()
    {
        var dto = new CreateSectionDto { Title = "Valid Title", Order = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}