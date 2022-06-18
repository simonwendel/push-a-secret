using Domain;
using Validation.General;

namespace Validation;

public class CiphertextValidator : ValidatorPipelineBase<string>, ICiphertextValidator
{
    private const int MaxCleartextLength = 72;
    private const int MaxBytesPerCodePoint = 2;
    private const int AesGcmTagByteLength = 16;

    internal static readonly int MaxCipherLength
        = Base64Statistics.GetInflationFor(MaxCleartextLength * MaxBytesPerCodePoint + AesGcmTagByteLength);

    public CiphertextValidator()
        : base(new NonEmptyStringLengthValidator(1, MaxCipherLength), new Base64AlphabetValidator())
    {
    }
}
