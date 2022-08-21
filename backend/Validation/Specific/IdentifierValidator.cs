// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Validation.General;

namespace Validation.Specific;

internal class IdentifierValidator : ValidatorPipelineBase<Identifier>, IIdentifierValidator
{
    public IdentifierValidator()
        : base(
            new ObjectPropertyValidator<Identifier, string>(x => x.Value, new NonEmptyStringLengthValidator(1, 13)), 
            new ObjectPropertyValidator<Identifier, string>(x => x.Value, new AsciiAlphaNumericStringValidator()))
    {
    }
}
