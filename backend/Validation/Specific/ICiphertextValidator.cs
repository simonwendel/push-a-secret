namespace Validation.Specific;

public interface ICiphertextValidator
{
    string Validate(UntrustedValue<string> untrustedValue);
}
