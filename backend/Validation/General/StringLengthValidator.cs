namespace Validation.General;

internal class StringLengthValidator : ValidatorBase<string>
{
    private readonly int minLength;
    private readonly int maxLength;

    public StringLengthValidator(int minLength, int maxLength = int.MaxValue)
    {
        if (minLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minLength));
        }

        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength));
        }

        if (maxLength < minLength)
        {
            throw new ArgumentException();
        }

        this.minLength = minLength;
        this.maxLength = maxLength;
    }

    private protected override bool Valid(string value)
    {
        return value.Length >= minLength && value.Length <= maxLength;
    }
}
