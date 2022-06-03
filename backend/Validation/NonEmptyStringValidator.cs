namespace Validation;

public class NonEmptyStringValidator : ValidatorBase<string>
{
    private protected override bool Valid(string value)
        => !string.IsNullOrWhiteSpace(value);
}
