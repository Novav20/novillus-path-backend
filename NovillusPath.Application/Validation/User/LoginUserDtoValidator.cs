using NovillusPath.Application.DTOs.User;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.User;

public class LoginUserDtoValidator : BaseValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("{PropertyName} es requerido.")
            .EmailAddress().WithMessage("Formato de correo electrónico inválido.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("{PropertyName} es requerido.");
    }
}
