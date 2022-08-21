// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Validation.General;

internal class NonEmptyStringLengthValidator : ValidatorPipelineBase<string>
{
    public NonEmptyStringLengthValidator(int minLength, int maxLength = int.MaxValue)
        : base(new NonEmptyStringValidator(), new StringLengthValidator(minLength, maxLength))
    {
    }
}
