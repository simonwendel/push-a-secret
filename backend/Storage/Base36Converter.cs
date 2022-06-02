namespace Storage;

public class Base36Converter
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

    private string ToBase36Recursive(long number, string results)
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
}
