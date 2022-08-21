// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Validation.General;
using Validation.Specific;

namespace Validation;

internal class Validator : IValidator
{
    private readonly IIdentifierValidator idValidator;
    private readonly ISecretValidator secretValidator;

    public Validator(IIdentifierValidator idValidator, ISecretValidator secretValidator)
    {
        this.idValidator = idValidator;
        this.secretValidator = secretValidator;
    }

    public T Validate<T>(UntrustedValue<T> untrustedValue) where T : notnull
        => SelectValidator<T>().Validate(untrustedValue);

    private IValidatorBase<T> SelectValidator<T>() where T : notnull
        => default(TokenOf<T>) switch
        {
            TokenOf<Secret> _ => secretValidator as IValidatorBase<T>,
            TokenOf<Identifier> _ => idValidator as IValidatorBase<T>,
            _ => null
        } ?? throw new InvalidOperationException();

    private struct TokenOf<T>
    {
    }
}
