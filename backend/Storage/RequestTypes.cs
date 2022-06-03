namespace Storage;

public enum Result
{
    OK,
    Err
}

public record Identifier(string Id);

public record Secret(string Algorithm, string IV, string Ciphertext);

public record OperationResult(Result Result);

public record IdentifierResult(Result Result, Identifier? Identifier);

public record SecretResult(Result Result, Secret? Secret);

public record PeekRequest(string Id) : Identifier(Id);

public record PeekResponse(Result Result) : OperationResult(Result);

public record CreateRequest(string Algorithm, string IV, string Ciphertext) : Secret(Algorithm, IV, Ciphertext);

public record CreateResponse(Result Result, Identifier? Identifier) : IdentifierResult(Result, Identifier);

public record ReadRequest(string Id) : Identifier(Id);

public record ReadResponse(Result Result, Secret? Secret) : SecretResult(Result, Secret);

public record DeleteRequest(string Id) : Identifier(Id);

public record DeleteResponse(Result Result) : OperationResult(Result);
