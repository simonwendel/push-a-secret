// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Validation.General;

namespace Validation.Specific;

internal class TtlDaysValidator : ValidatorPipelineBase<int>, ITtlDaysValidator
{
    public const int MinTtl = 1;
    public const int MaxTtl = 7;

    public TtlDaysValidator()
        : base(new IntegerIntervalValidator(MinTtl, MaxTtl))
    {
    }
}
