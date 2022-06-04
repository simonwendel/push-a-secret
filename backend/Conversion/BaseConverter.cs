namespace Conversion;

public class BaseConverter : IBaseConverter
{
    private const string alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";
    private const int radix = 36;

    public string ToBase36(long number)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number));
        }

        return ToBase36Recursive(number, string.Empty);
    }

    private static string ToBase36Recursive(long number, string results)
    {
        var quote = number / radix;
        var remainder = number % radix;
        var digit = alphabet[(int) remainder];
        if (quote <= 0)
        {
            return digit + results;
        }

        return ToBase36Recursive(quote, digit + results);
    }

    public long FromBase36(string base36)
    {
        if (string.IsNullOrWhiteSpace(base36) || !base36.All(char.IsLetterOrDigit))
        {
            throw new ArgumentException();
        }

        base36 = base36.ToLowerInvariant();
        return FromBase36Recursive(base36.ToCharArray(), 0);
    }

    private static long FromBase36Recursive(char[] base36, long results)
    {
        if (base36.Length == 0)
        {
            return results;
        }

        var currentDigit = base36[0];
        var digitValue = alphabet.IndexOf(currentDigit);
        return FromBase36Recursive(base36[1..], results * radix + digitValue);
    }
}
