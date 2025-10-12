using FluentValidation.TestHelper;
using NovillusPath.Application.DTOs.User;
using NovillusPath.Application.Validation.User;

namespace NovillusPath.Test.ValidationTests.User;

public class RegisterUserDtoValidatorTests
{
    private readonly RegisterUserDtoValidator _validator;

    public RegisterUserDtoValidatorTests()
    {
        _validator = new RegisterUserDtoValidator();
    }

    [Fact]
    public void Email_ShouldNotBeEmpty()
    {
        var dto = new RegisterUserDto { Email = "", Password = "Password123!", ConfirmPassword = "Password123!" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email es requerido.");
    }

    [Fact]
    public void Email_ShouldBeValidFormat()
    {
        var dto = new RegisterUserDto { Email = "invalid-email", Password = "Password123!", ConfirmPassword = "Password123!" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email no es una dirección de correo electrónico válida.");
    }

    [Fact]
    public void Password_ShouldNotBeEmpty()
    {
        var dto = new RegisterUserDto { Email = "test@example.com", Password = "", ConfirmPassword = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage("Password es requerido.");
    }

    [Fact]
    public void Password_ShouldBeAtLeast6CharactersLong()
    {
        var dto = new RegisterUserDto { Email = "test@example.com", Password = "Pass1", ConfirmPassword = "Pass1" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage("Password debe tener al menos 6 caracteres.");
    }

    [Fact]
    public void ConfirmPassword_ShouldNotBeEmpty()
    {
        var dto = new RegisterUserDto { Email = "test@example.com", Password = "Password123!", ConfirmPassword = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
              .WithErrorMessage("Confirm Password es requerido.");
    }

    [Fact]
    public void ConfirmPassword_ShouldMatchPassword()
    {
        var dto = new RegisterUserDto { Email = "test@example.com", Password = "Password123!", ConfirmPassword = "Mismatch123!" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
              .WithErrorMessage("Las contraseñas no coinciden.");
    }

    [Fact]
    public void FullName_ShouldNotExceedMaxLength_WhenProvided()
    {
        var dto = new RegisterUserDto { Email = "test@example.com", Password = "Password123!", ConfirmPassword = "Password123!", FullName = new string('a', 101) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
              .WithErrorMessage("Full Name no debe exceder los 100 caracteres.");
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var dto = new RegisterUserDto { Email = "test@example.com", Password = "Password123!", ConfirmPassword = "Password123!", FullName = "Test User" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Dto_ShouldBeValid_WhenFullNameIsNull()
    {
        var dto = new RegisterUserDto { Email = "test@example.com", Password = "Password123!", ConfirmPassword = "Password123!", FullName = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}