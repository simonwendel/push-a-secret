namespace Domain;

public interface IStore
{
    Result Peek(Identifier identifier);
    IdentifierResult Create(Secret secret);
    SecretResult Read(Identifier identifier);
    Result Delete(Identifier identifier);
}
