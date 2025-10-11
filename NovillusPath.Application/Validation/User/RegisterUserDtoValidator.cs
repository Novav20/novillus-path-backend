using NovillusPath.Application.DTOs.User;

namespace NovillusPath.Application.Validation.User;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .EmailAddress().WithMessage("{PropertyName} is not a valid email address.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MinimumLength(6).WithMessage("{PropertyName} must be at least 6 characters long.");

        RuleFor(u => u.ConfirmPassword)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Equal(u => u.Password).WithMessage("Passwords do not match.");

        RuleFor(u => u.FullName)
            .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.")
            .When(u => !string.IsNullOrEmpty(u.FullName));
    }
}
