using System.Linq.Expressions;

namespace Validation.General;

internal class ObjectPropertyValidator<T, TProp> : ValidatorBase<T>
    where T : notnull
    where TProp : notnull
{
    private readonly Expression<Func<T, TProp>> selector;
    private readonly IValidatorBase<TProp> validator;

    public ObjectPropertyValidator(Expression<Func<T, TProp>> selector, IValidatorBase<TProp> validator)
    {
        this.selector = selector;
        this.validator = validator;
    }

    private protected override bool Valid(T value)
    {
        try
        {
            validator.Validate(new UntrustedValue<TProp>(selector.Compile().Invoke(value)));
            return true;
        }
        catch (ValidationException)
        {
            return false;
        }
    }
}
