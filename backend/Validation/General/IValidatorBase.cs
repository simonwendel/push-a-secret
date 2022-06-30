namespace Validation.General;

internal interface IValidatorBase<T> where T : notnull
{
    T Validate(UntrustedValue<T> untrustedValue);
}
