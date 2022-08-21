// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Validation.General;

namespace Validation.Specific;

internal class CiphertextValidator : ValidatorPipelineBase<string>, ICiphertextValidator
{
    private const int MaxCleartextLength = 72;
    private const int MinCleartextLength = 1;
    
    private const int MaxBytesPerCodePoint = 2;
    
    private const int AesGcmTagByteLength = 16;

    internal static readonly int MaxCipherLength
        = Base64Statistics.GetInflationFor(MaxCleartextLength * MaxBytesPerCodePoint + AesGcmTagByteLength);

    internal static readonly int MinCipherLength
        = Base64Statistics.GetInflationFor(MinCleartextLength * MaxBytesPerCodePoint + AesGcmTagByteLength);

    public CiphertextValidator()
        : base(new NonEmptyStringLengthValidator(MinCipherLength, MaxCipherLength), new Base64AlphabetValidator())
    {
    }
}
