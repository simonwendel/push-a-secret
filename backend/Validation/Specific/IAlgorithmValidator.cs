namespace Validation.Specific;

public interface IAlgorithmValidator
{
    string Validate(UntrustedValue<string> untrustedValue);
}
