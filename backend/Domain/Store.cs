namespace Domain;

internal class Store : IStore
{
    private readonly IRepository repository;
    private readonly IIdGenerator idGenerator;

    public Store(IRepository repository, IIdGenerator idGenerator)
    {
        this.repository = repository;
        this.idGenerator = idGenerator;
    }

    public Result Peek(Identifier identifier) 
        => repository.Peek(identifier);

    public IdentifierResult Create(Secret secret)
    {
        var identifier = idGenerator.Generate();
        return repository.Create(identifier, secret) switch
        {
            Result.OK => new IdentifierResult(Result.OK, identifier),
            _ => new IdentifierResult(Result.Err, null)
        };
    }

    public SecretResult Read(Identifier identifier)
        => repository.Read(identifier) switch
        {
            (Result.OK, { } secret) => new SecretResult(Result.OK, secret),
            _ => new SecretResult(Result.Err, null)
        };

    public Result Delete(Identifier identifier) 
        => repository.Delete(identifier);
}
