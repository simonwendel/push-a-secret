using Domain;

namespace Validation.Specific;

public interface IIdentifierValidator
{
    Identifier Validate(UntrustedValue<Identifier> untrustedValue);
}
