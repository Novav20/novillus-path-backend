namespace NovillusPath.Application.Validation.Common;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected IRuleBuilder<T, string> RuleForRequiredString(System.Linq.Expressions.Expression<Func<T, string>> expression, int minLength = 3, int maxLength = 100)
    {
        return RuleFor(expression)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MinimumLength(minLength).WithMessage("{PropertyName} must be at least {MinLength} characters long.")
            .MaximumLength(maxLength).WithMessage("{PropertyName} cannot exceed {MaxLength} characters.");
    }

    protected IRuleBuilder<T, string?> RuleForOptionalString(System.Linq.Expressions.Expression<Func<T, string?>> expression, int maxLength = 1000)
    {
        return RuleFor(expression)
            .MaximumLength(maxLength).WithMessage("{PropertyName} cannot exceed {MaxLength} characters.")
            .When(x => !string.IsNullOrEmpty(expression.Compile().Invoke(x)));
    }

    protected IRuleBuilder<T, decimal> RuleForRequiredDecimal(System.Linq.Expressions.Expression<Func<T, decimal>> expression, decimal minValue = 0)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(minValue).WithMessage("{PropertyName} must be greater than or equal to {MinValue}.");
    }

    protected IRuleBuilder<T, decimal?> RuleForOptionalDecimal(System.Linq.Expressions.Expression<Func<T, decimal?>> expression, decimal minValue = 0)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(minValue).WithMessage("{PropertyName} must be greater than or equal to {MinValue}.")
            .When(x => expression.Compile().Invoke(x).HasValue);
    }

    protected IRuleBuilder<T, int> RuleForRequiredInteger(System.Linq.Expressions.Expression<Func<T, int>> expression, int minValue = 0)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(minValue).WithMessage("{PropertyName} must be greater than or equal to {MinValue}.");
    }

    protected IRuleBuilder<T, int?> RuleForOptionalInteger(System.Linq.Expressions.Expression<Func<T, int?>> expression, int minValue = 0)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(minValue).WithMessage("{PropertyName} must be greater than or equal to {MinValue}.")
            .When(x => expression.Compile().Invoke(x).HasValue);
    }

    protected IRuleBuilder<T, byte> RuleForRequiredByte(System.Linq.Expressions.Expression<Func<T, byte>> expression, byte minValue = 1, byte maxValue = 5)
    {
        return RuleFor(expression)
            .InclusiveBetween(minValue, maxValue).WithMessage("{PropertyName} must be between {MinValue} and {MaxValue}.");
    }

    protected IRuleBuilder<T, byte?> RuleForOptionalByte(System.Linq.Expressions.Expression<Func<T, byte?>> expression, byte minValue = 1, byte maxValue = 5)
    {
        return RuleFor(expression)
            .InclusiveBetween(minValue, maxValue).WithMessage("{PropertyName} must be between {MinValue} and {MaxValue}.")
            .When(x => expression.Compile().Invoke(x).HasValue);
    }

    protected IRuleBuilder<T, DateTime?> RuleForOptionalFutureDate(System.Linq.Expressions.Expression<Func<T, DateTime?>> expression)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("{PropertyName} must be a future date.")
            .When(x => expression.Compile().Invoke(x).HasValue);
    }

    protected IRuleBuilder<T, Guid> RuleForRequiredGuid(System.Linq.Expressions.Expression<Func<T, Guid>> expression)
    {
        return RuleFor(expression)
            .NotEmpty().WithMessage("{PropertyName} is required and cannot be an empty GUID.");
    }

    protected IRuleBuilder<T, List<Guid>?> RuleForOptionalGuidList(System.Linq.Expressions.Expression<Func<T, List<Guid>?>> expression)
    {
        return RuleFor(expression)
            .Must(list => list == null || list.All(id => id != Guid.Empty)).WithMessage("The IDs in the list cannot be empty GUIDs.")
            .When(x => expression.Compile().Invoke(x) != null);
    }
}