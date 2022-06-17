using Validation.General;

namespace Validation;

public class IdentifierValidator : ValidatorPipelineBase<string>, IIdentifierValidator
{
    public IdentifierValidator()
        : base(new NonEmptyStringLengthValidator(1, 13), new AsciiAlphaNumericStringValidator())
    {
    }
}
