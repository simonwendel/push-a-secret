namespace Validation.Specific;

public interface IIvValidator
{
    string Validate(UntrustedValue<string> untrustedValue);
}
