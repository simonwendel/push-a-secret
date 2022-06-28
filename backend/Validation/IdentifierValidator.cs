using Domain;
using Validation.General;

namespace Validation;

public class IdentifierValidator : ValidatorPipelineBase<Identifier>, IIdentifierValidator
{
    public IdentifierValidator()
        : base(
            new ObjectPropertyValidator<Identifier, string>(x => x.Value, new NonEmptyStringLengthValidator(1, 13)), 
            new ObjectPropertyValidator<Identifier, string>(x => x.Value, new AsciiAlphaNumericStringValidator()))
    {
    }
}
