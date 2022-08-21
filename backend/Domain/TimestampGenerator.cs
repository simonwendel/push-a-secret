// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Domain;

internal class TimestampGenerator : ITimestampGenerator
{
    public long Generate()
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
