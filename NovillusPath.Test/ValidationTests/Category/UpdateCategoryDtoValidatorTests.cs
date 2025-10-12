using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.Category;
using NovillusPath.Application.Validation.Category;

namespace NovillusPath.Test.ValidationTests.Category;

public class UpdateCategoryDtoValidatorTests
{
    private readonly UpdateCategoryDtoValidator _validator;

    public UpdateCategoryDtoValidatorTests()
    {
        _validator = new UpdateCategoryDtoValidator();
    }

    [Fact]
    public void Name_ShouldNotBeEmpty_WhenProvided()
    {
        var dto = new UpdateCategoryDto { Name = "", Description = "Valid Description" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name es requerido si se proporciona.");
    }

    [Fact]
    public void Name_ShouldNotExceedMaxLength_WhenProvided()
    {
        var dto = new UpdateCategoryDto { Name = new string('a', 101), Description = "Valid Description" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name no debe exceder los 100 caracteres.");
    }

    [Fact]
    public void Name_ShouldBeAtLeastMinLength_WhenProvided()
    {
        var dto = new UpdateCategoryDto { Name = "ab", Description = "Valid Description" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name debe tener al menos 3 caracteres.");
    }

    [Fact]
    public void Description_ShouldNotExceedMaxLength_WhenProvided()
    {
        var dto = new UpdateCategoryDto { Name = "Valid Name", Description = new string('a', 501) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description no debe exceder los 500 caracteres.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new UpdateCategoryDto { Name = "Valid Name", Description = "Valid Description" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenOnlyNameIsProvided()
    {
        var dto = new UpdateCategoryDto { Name = "Valid Name" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenOnlyDescriptionIsProvided()
    {
        var dto = new UpdateCategoryDto { Description = "Valid Description" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenNoPropertiesAreProvided()
    {
        var dto = new UpdateCategoryDto { };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}