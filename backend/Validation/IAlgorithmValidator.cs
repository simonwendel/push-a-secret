namespace Validation;

public interface IAlgorithmValidator
{
    string Validate(UntrustedValue<string> untrustedValue);
}
