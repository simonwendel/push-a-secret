using Domain;

namespace Validation;

public interface IIdentifierValidator
{
    Identifier Validate(UntrustedValue<Identifier> untrustedValue);
}
