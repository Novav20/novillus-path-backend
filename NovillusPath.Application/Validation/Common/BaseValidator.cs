namespace NovillusPath.Application.Validation.Common;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected IRuleBuilder<T, string> RuleForRequiredString(System.Linq.Expressions.Expression<Func<T, string>> expression, int minLength = 3, int maxLength = 100)
    {
        return RuleFor(expression)
            .NotEmpty().WithMessage("{PropertyName} es requerido.")
            .MinimumLength(minLength).WithMessage("{PropertyName} debe tener al menos {MinLength} caracteres.")
            .MaximumLength(maxLength).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres.");
    }

    protected IRuleBuilder<T, string?> RuleForOptionalString(System.Linq.Expressions.Expression<Func<T, string?>> expression, int maxLength = 1000)
    {
        return RuleFor(expression)
            .MaximumLength(maxLength).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres.")
            .When(x => !string.IsNullOrEmpty(expression.Compile().Invoke(x)));
    }

    protected IRuleBuilder<T, decimal> RuleForRequiredDecimal(System.Linq.Expressions.Expression<Func<T, decimal>> expression, decimal minValue = 0)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(minValue).WithMessage("{PropertyName} debe ser mayor o igual a {MinValue}.");
    }

    protected IRuleBuilder<T, decimal?> RuleForOptionalDecimal(System.Linq.Expressions.Expression<Func<T, decimal?>> expression, decimal minValue = 0)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(minValue).WithMessage("{PropertyName} debe ser mayor o igual a {MinValue}.")
            .When(x => expression.Compile().Invoke(x).HasValue);
    }

    protected IRuleBuilder<T, int> RuleForRequiredInteger(System.Linq.Expressions.Expression<Func<T, int>> expression, int minValue = 0)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(minValue).WithMessage("{PropertyName} debe ser mayor o igual a {MinValue}.");
    }

    protected IRuleBuilder<T, int?> RuleForOptionalInteger(System.Linq.Expressions.Expression<Func<T, int?>> expression, int minValue = 0)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(minValue).WithMessage("{PropertyName} debe ser mayor o igual a {MinValue}.")
            .When(x => expression.Compile().Invoke(x).HasValue);
    }

    protected IRuleBuilder<T, byte> RuleForRequiredByte(System.Linq.Expressions.Expression<Func<T, byte>> expression, byte minValue = 1, byte maxValue = 5)
    {
        return RuleFor(expression)
            .InclusiveBetween(minValue, maxValue).WithMessage("{PropertyName} debe estar entre {MinValue} y {MaxValue}.");
    }

    protected IRuleBuilder<T, byte?> RuleForOptionalByte(System.Linq.Expressions.Expression<Func<T, byte?>> expression, byte minValue = 1, byte maxValue = 5)
    {
        return RuleFor(expression)
            .InclusiveBetween(minValue, maxValue).WithMessage("{PropertyName} debe estar entre {MinValue} y {MaxValue}.")
            .When(x => expression.Compile().Invoke(x).HasValue);
    }

    protected IRuleBuilder<T, DateTime?> RuleForOptionalFutureDate(System.Linq.Expressions.Expression<Func<T, DateTime?>> expression)
    {
        return RuleFor(expression)
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("{PropertyName} debe ser una fecha futura.")
            .When(x => expression.Compile().Invoke(x).HasValue);
    }

    protected IRuleBuilder<T, Guid> RuleForRequiredGuid(System.Linq.Expressions.Expression<Func<T, Guid>> expression)
    {
        return RuleFor(expression)
            .NotEmpty().WithMessage("{PropertyName} es requerido y no puede ser un GUID vacío.");
    }

    protected IRuleBuilder<T, List<Guid>?> RuleForOptionalGuidList(System.Linq.Expressions.Expression<Func<T, List<Guid>?>> expression)
    {
        return RuleFor(expression)
            .Must(list => list == null || list.All(id => id != Guid.Empty)).WithMessage("Los IDs en la lista no pueden ser GUIDs vacíos.")
            .When(x => expression.Compile().Invoke(x) != null);
    }
}