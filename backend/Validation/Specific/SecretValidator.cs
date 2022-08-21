// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Validation.General;

namespace Validation.Specific;

internal class SecretValidator : ValidatorPipelineBase<Secret>, ISecretValidator
{
    public SecretValidator(
        IAlgorithmValidator algorithmValidator,
        IIvValidator ivValidator,
        ICiphertextValidator ciphertextValidator,
        ITtlDaysValidator ttlDaysValidator)
        : base(
            new ObjectPropertyValidator<Secret, string>(x => x.Algorithm, algorithmValidator),
            new ObjectPropertyValidator<Secret, string>(x => x.IV, ivValidator),
            new ObjectPropertyValidator<Secret, string>(x => x.Ciphertext, ciphertextValidator),
            new ObjectPropertyValidator<Secret, int>(x => x.Ttl, ttlDaysValidator))
    {
    }
}
