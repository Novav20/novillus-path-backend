using NovillusPath.Application.DTOs.User;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.User;

public class RegisterUserDtoValidator : BaseValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("{PropertyName} es requerido.")
            .EmailAddress().WithMessage("{PropertyName} no es una dirección de correo electrónico válida.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("{PropertyName} es requerido.")
            .MinimumLength(6).WithMessage("{PropertyName} debe tener al menos 6 caracteres.");

        RuleFor(u => u.ConfirmPassword)
            .NotEmpty().WithMessage("{PropertyName} es requerido.")
            .Equal(u => u.Password).WithMessage("Las contraseñas no coinciden.");

        RuleForOptionalString(u => u.FullName, 100);
    }
}
