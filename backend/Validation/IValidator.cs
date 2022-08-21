// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Validation;

public interface IValidator
{
    T Validate<T>(UntrustedValue<T> untrustedValue) where T : notnull;
}