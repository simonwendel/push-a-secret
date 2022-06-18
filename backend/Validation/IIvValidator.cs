namespace Validation;

public interface IIvValidator
{
    string Validate(UntrustedValue<string> untrustedValue);
}
