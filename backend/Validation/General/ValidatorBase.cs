﻿namespace Validation.General;

internal abstract class ValidatorBase<T> : IValidatorBase<T> where T : notnull
{
    public T Validate(UntrustedValue<T> untrustedValue)
        => Valid(untrustedValue.Value)
            ? untrustedValue.Value
            : throw new ValidationException();

    private protected abstract bool Valid(T value);
}
