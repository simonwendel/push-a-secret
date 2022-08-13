namespace Validation.General;

internal class StringLengthValidator : ValidatorPipelineBase<string>
{
    public StringLengthValidator(int minLength, int maxLength = int.MaxValue)
        : base(
            new ObjectPropertyValidator<string, int>(
                x => x.Length,
                new IntegerIntervalValidator(minLength, maxLength)))
    {
    }
}
