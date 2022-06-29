using Domain;

namespace Validation.Specific;

public interface ISecretValidator
{
    Secret Validate(UntrustedValue<Secret> untrustedValue);
}
