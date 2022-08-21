// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Validation.General;

internal class IntegerIntervalValidator : ValidatorBase<int>
{
    private readonly int lowerBounds;
    private readonly int upperBounds;

    public IntegerIntervalValidator(int lowerBounds, int upperBounds = int.MaxValue)
    {
        if (lowerBounds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lowerBounds));
        }

        if (upperBounds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(upperBounds));
        }

        if (upperBounds < lowerBounds)
        {
            throw new ArgumentException();
        }

        this.lowerBounds = lowerBounds;
        this.upperBounds = upperBounds;
    }

    private protected override bool Valid(int value)
    {
        return value >= lowerBounds && value <= upperBounds;
    }
}
