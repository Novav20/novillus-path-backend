using NovillusPath.Application.DTOs.User;
using NovillusPath.Application.Validation.Common;

namespace NovillusPath.Application.Validation.User;

public class RegisterUserDtoValidator : BaseValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .EmailAddress().WithMessage("{PropertyName} is not a valid email address.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MinimumLength(6).WithMessage("{PropertyName} must have at least 6 characters.");

        RuleFor(u => u.ConfirmPassword)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Equal(u => u.Password).WithMessage("Passwords do not match.");

        RuleForOptionalString(u => u.FullName, 100);
    }
}
