// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Validation.General;

internal interface IValidatorBase<T> where T : notnull
{
    T Validate(UntrustedValue<T> untrustedValue);
}
