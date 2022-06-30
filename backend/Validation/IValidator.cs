namespace Validation;

public interface IValidator
{
    T Validate<T>(UntrustedValue<T> untrustedValue) where T : notnull;
}