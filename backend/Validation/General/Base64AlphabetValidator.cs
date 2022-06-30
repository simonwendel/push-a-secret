namespace Validation.General;

internal class Base64AlphabetValidator : ValidatorBase<string>
{
    private protected override bool Valid(string value)
        => Convert.TryFromBase64String(value, new byte[value.Length * 2], out _); // BMP is max 2 bytes per character
}
