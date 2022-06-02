namespace Storage;

public class TimestampGenerator : ITimestampGenerator
{
    public long Generate() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
