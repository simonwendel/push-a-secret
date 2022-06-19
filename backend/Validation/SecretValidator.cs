using Domain;
using Validation.General;

namespace Validation;

public class SecretValidator : ValidatorPipelineBase<Secret>, ISecretValidator
{
    public SecretValidator()
        : base(
            new ObjectPropertyValidator<Secret, string>(x => x.Algorithm, new AlgorithmValidator()),
            new ObjectPropertyValidator<Secret, string>(x => x.IV, new IvValidator()),
            new ObjectPropertyValidator<Secret, string>(x => x.Ciphertext, new CiphertextValidator()))
    {
    }
}
