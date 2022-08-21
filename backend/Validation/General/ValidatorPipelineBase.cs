// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Validation.General;

internal abstract class ValidatorPipelineBase<T> : ValidatorBase<T> where T : notnull
{
    private readonly ValidatorBase<T>[] validators;

    private protected ValidatorPipelineBase(params ValidatorBase<T>[] validators)
    {
        if (validators.Length == 0)
        {
            throw new ArgumentException("A pipeline without validators is pretty useless.");
        }
        
        this.validators = validators;
    }

    private protected override bool Valid(T value)
        => validators.All(validator =>
        {
            validator.Validate(new UntrustedValue<T>(value));
            return true;
        });
}
