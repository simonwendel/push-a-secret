﻿using Conversion;
using Validation.General;

namespace Validation;

public class IvValidator : ValidatorPipelineBase<string>
{
    private const int IvNumberOfBytes = 12;

    internal static readonly int IvLength
        = Base64Statistics.GetInflationFor(IvNumberOfBytes);
    
    public IvValidator()
        : base(new NonEmptyStringLengthValidator(IvLength, IvLength), new Base64AlphabetValidator())
    {
    }
}
