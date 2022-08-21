// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Domain;

public interface IBaseConverter
{
    string ToBase36(long number);
    long FromBase36(string base36);
}
