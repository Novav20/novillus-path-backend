using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.Category;
using NovillusPath.Application.Validation.Category;

namespace NovillusPath.Test.ValidationTests.Category;

public class CreateCategoryDtoValidatorTests
{
    private readonly CreateCategoryDtoValidator _validator;

    public CreateCategoryDtoValidatorTests()
    {
        _validator = new CreateCategoryDtoValidator();
    }

    [Fact]
    public void Name_ShouldNotBeEmpty()
    {
        var dto = new CreateCategoryDto { Name = "", Description = "Valid Description" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name es requerido.");
    }

    [Fact]
    public void Name_ShouldNotExceedMaxLength()
    {
        var dto = new CreateCategoryDto { Name = new string('a', 101), Description = "Valid Description" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name no debe exceder los 100 caracteres.");
    }

    [Fact]
    public void Name_ShouldBeAtLeastMinLength()
    {
        var dto = new CreateCategoryDto { Name = "ab", Description = "Valid Description" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name debe tener al menos 3 caracteres.");
    }

    [Fact]
    public void Description_ShouldNotExceedMaxLength()
    {
        var dto = new CreateCategoryDto { Name = "Valid Name", Description = new string('a', 501) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description no debe exceder los 500 caracteres.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new CreateCategoryDto { Name = "Valid Name", Description = "Valid Description" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenDescriptionIsNull()
    {
        var dto = new CreateCategoryDto { Name = "Valid Name", Description = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}