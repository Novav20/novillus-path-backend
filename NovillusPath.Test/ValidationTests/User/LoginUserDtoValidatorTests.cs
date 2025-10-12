using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.User;
using NovillusPath.Application.Validation.User;

namespace NovillusPath.Test.ValidationTests.User;

public class LoginUserDtoValidatorTests
{
    private readonly LoginUserDtoValidator _validator;

    public LoginUserDtoValidatorTests()
    {
        _validator = new LoginUserDtoValidator();
    }

    [Fact]
    public void Email_ShouldNotBeEmpty()
    {
        var dto = new LoginUserDto { Email = "", Password = "password" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email es requerido.");
    }

    [Fact]
    public void Email_ShouldBeValidFormat()
    {
        var dto = new LoginUserDto { Email = "invalid-email", Password = "password" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Formato de correo electrónico inválido.");
    }

    [Fact]
    public void Password_ShouldNotBeEmpty()
    {
        var dto = new LoginUserDto { Email = "test@example.com", Password = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage("Password es requerido.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new LoginUserDto { Email = "test@example.com", Password = "password" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}