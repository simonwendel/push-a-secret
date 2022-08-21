// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Validation.General;

namespace Validation.Specific;

internal class AlgorithmValidator : ValidatorPipelineBase<string>, IAlgorithmValidator
{
    public AlgorithmValidator()
        : base(new NonEmptyStringValidator())
    {
    }

    public const string DefaultAlgorithm = "A128GCM";

    private protected override bool Valid(string value) 
        => base.Valid(value) && value.Equals(DefaultAlgorithm);
}
