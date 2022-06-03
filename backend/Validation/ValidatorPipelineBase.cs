namespace Validation;

public abstract class ValidatorPipelineBase<T> : ValidatorBase<T>
{
    private readonly ValidatorBase<T>[] validators;

    private protected ValidatorPipelineBase(params ValidatorBase<T>[] validators)
    {
        this.validators = validators;
    }

    private protected override bool Valid(T value)
        => validators.All(validator =>
        {
            validator.Validate(new UntrustedValue<T>(value));
            return true;
        });
}
