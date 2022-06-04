namespace Conversion;

public interface IBaseConverter
{
    string ToBase36(long number);
    long FromBase36(string base36);
}
