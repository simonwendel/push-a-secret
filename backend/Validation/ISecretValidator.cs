using Domain;

namespace Validation;

public interface ISecretValidator
{
    Secret Validate(UntrustedValue<Secret> untrustedValue);
}
