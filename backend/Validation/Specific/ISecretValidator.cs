﻿// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Validation.General;

namespace Validation.Specific;

internal interface ISecretValidator : IValidatorBase<Secret>
{
}
