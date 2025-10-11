using NovillusPath.Application.DTOs.User;

namespace NovillusPath.Application.Validation.User;

public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
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
