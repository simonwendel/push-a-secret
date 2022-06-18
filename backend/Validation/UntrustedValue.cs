namespace Validation;

public class UntrustedValue<T> where T : notnull
{
    public UntrustedValue(T value)
    {
        Value = value;
    }

    internal T Value { get; }

    public sealed override bool Equals(object? obj)
        => Value.Equals(obj);

    public bool Equals(T other) 
        => Equals((object?) other);

    /// <remarks>
    /// The GetHashCode() method can of course be used to reverse-engineer the underlying
    /// value without validating it first, but there are easier ways than this if you really
    /// want to, f.x. reflection.
    /// </remarks>
    public sealed override int GetHashCode() 
        => Value.GetHashCode();

    /// <remarks>
    /// In order to at least make it hard to bypass validation, ToString() will always throw an
    /// exception and is sealed to prevent such silliness.
    /// </remarks>>
    /// <exception cref="InvalidOperationException">Always.</exception>
    public sealed override string ToString()
        => throw new InvalidOperationException("Bypassing validation by calling ToString() is not allowed.");
}
