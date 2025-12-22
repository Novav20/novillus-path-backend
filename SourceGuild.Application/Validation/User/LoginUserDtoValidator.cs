using SourceGuild.Application.DTOs.User;
using SourceGuild.Application.Validation.Common;

namespace SourceGuild.Application.Validation.User;

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
