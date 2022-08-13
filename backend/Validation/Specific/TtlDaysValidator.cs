using Validation.General;

namespace Validation.Specific;

internal class TtlDaysValidator : ValidatorPipelineBase<int>
{
    public const int MinTtl = 1;
    public const int MaxTtl = 7;

    public TtlDaysValidator()
        : base(new IntegerIntervalValidator(MinTtl, MaxTtl))
    {
    }
}
