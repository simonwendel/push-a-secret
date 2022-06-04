namespace Validation;

public interface IIdentifierValidator
{
    string Validate(UntrustedValue<string> untrustedValue);
}
