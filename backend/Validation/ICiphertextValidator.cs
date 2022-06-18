namespace Validation;

public interface ICiphertextValidator
{
    string Validate(UntrustedValue<string> untrustedValue);
}
