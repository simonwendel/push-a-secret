// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Validation.General;

internal class AsciiAlphaNumericStringValidator : ValidatorBase<string>
{
    private protected override bool Valid(string value) 
        => value.All(char.IsAscii) && value.All(char.IsLetterOrDigit);
}
