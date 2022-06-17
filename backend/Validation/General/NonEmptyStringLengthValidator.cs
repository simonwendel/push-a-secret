﻿namespace Validation.General;

internal class NonEmptyStringLengthValidator : ValidatorPipelineBase<string>
{
    public NonEmptyStringLengthValidator(int minLength, int maxLength = int.MaxValue)
        : base(new NonEmptyStringValidator(), new StringLengthValidator(minLength, maxLength))
    {
    }
}