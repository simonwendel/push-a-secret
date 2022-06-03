namespace Storage;

internal class IdGenerator : IIdGenerator
{
    private readonly ITimestampGenerator timestampGenerator;
    private readonly IBase36Converter baseConverter;

    public IdGenerator(ITimestampGenerator timestampGenerator, IBase36Converter baseConverter)
    {
        this.timestampGenerator = timestampGenerator;
        this.baseConverter = baseConverter;
    }
    
    public Identifier Generate()
    {
        var timestamp = timestampGenerator.Generate();
        var base36 = baseConverter.ToBase36(timestamp);
        return new Identifier(base36);
    }
}
