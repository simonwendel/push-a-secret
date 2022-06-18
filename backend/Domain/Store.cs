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

    public PeekResponse Peek(PeekRequest request)
    {
        var result = repository.Peek(request);
        return new PeekResponse(result);
    }

    public CreateResponse Create(CreateRequest request)
    {
        var id = idGenerator.Generate();
        return repository.Create(id, request) switch
        {
            Result.OK => new CreateResponse(Result.OK, id),
            _ => new CreateResponse(Result.Err, null)
        };
    }

    public ReadResponse Read(ReadRequest request)
        => repository.Read(request) switch
        {
            (Result.OK, { } secret) => new ReadResponse(Result.OK, secret),
            _ => new ReadResponse(Result.Err, null)
        };

    public DeleteResponse Delete(DeleteRequest request)
    {
        var result = repository.Delete(request);
        return new DeleteResponse(result);
    }
}
