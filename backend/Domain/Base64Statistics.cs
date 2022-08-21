// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Domain;

public static class Base64Statistics
{
    public static int GetInflationFor(int numberOfBytes)
    {
        if (numberOfBytes is < 0 or > int.MaxValue / 2)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfBytes));
        }
        
        if (numberOfBytes % 3 == 0)
        {
            return numberOfBytes / 3 * 4;
        }

        return numberOfBytes / 3 * 4 + 4;
    }
}
