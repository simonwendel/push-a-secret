namespace Validation;

public class UntrustedValue<T>
{
    public UntrustedValue(T value)
    {
        Value = value;
    }

    internal T Value { get; }
}
