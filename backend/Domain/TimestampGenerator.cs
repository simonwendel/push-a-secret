namespace Domain;

internal class TimestampGenerator : ITimestampGenerator
{
    public long Generate()
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
