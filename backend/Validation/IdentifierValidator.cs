namespace Validation;

public class IdentifierValidator : ValidatorPipelineBase<string>
{
    public IdentifierValidator()
        : base(new NonEmptyStringLengthValidator(1, 13), new AsciiAlphaNumericStringValidator())
    {
    }
}
