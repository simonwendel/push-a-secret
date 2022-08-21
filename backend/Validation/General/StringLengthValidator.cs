// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

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
