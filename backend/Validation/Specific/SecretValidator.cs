using Domain;
using Validation.General;

namespace Validation.Specific;

internal class SecretValidator : ValidatorPipelineBase<Secret>, ISecretValidator
{
    public SecretValidator()
        : base(
            new ObjectPropertyValidator<Secret, string>(x => x.Algorithm, new AlgorithmValidator()),
            new ObjectPropertyValidator<Secret, string>(x => x.IV, new IvValidator()),
            new ObjectPropertyValidator<Secret, string>(x => x.Ciphertext, new CiphertextValidator()),
            new ObjectPropertyValidator<Secret, int>(x => x.Ttl, new TtlDaysValidator()))
    {
    }
}
