namespace Domain;

public interface IRepository
{
    public Result Peek(Identifier id);
    public Result Create(Identifier id, Secret secret);
    public (Result result, Secret? secret) Read(Identifier id);
    public Result Delete(Identifier id);
}
