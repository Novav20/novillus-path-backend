using NovillusPath.Application.DTOs.User;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.User;

public class LoginUserDtoValidator : BaseValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("{PropertyName} is required.");
    }
}
