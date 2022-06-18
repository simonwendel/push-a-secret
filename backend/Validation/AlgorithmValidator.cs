using Validation.General;

namespace Validation;

public class AlgorithmValidator : ValidatorPipelineBase<string>, IAlgorithmValidator
{
    public AlgorithmValidator()
        : base(new NonEmptyStringValidator())
    {
    }

    public const string DefaultAlgorithm = "A128GCM";

    private protected override bool Valid(string value) 
        => base.Valid(value) && value.Equals(DefaultAlgorithm);
}
