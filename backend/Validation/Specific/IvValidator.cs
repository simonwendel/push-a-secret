// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Validation.General;

namespace Validation.Specific;

internal class IvValidator : ValidatorPipelineBase<string>, IIvValidator
{
    private const int IvNumberOfBytes = 12;

    internal static readonly int IvLength
        = Base64Statistics.GetInflationFor(IvNumberOfBytes);

    public IvValidator()
        : base(new NonEmptyStringLengthValidator(IvLength, IvLength), new Base64AlphabetValidator())
    {
    }
}
