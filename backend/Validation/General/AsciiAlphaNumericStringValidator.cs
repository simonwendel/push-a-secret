namespace Validation.General;

internal class AsciiAlphaNumericStringValidator : ValidatorBase<string>
{
    private protected override bool Valid(string value) 
        => value.All(char.IsAscii) && value.All(char.IsLetterOrDigit);
}
