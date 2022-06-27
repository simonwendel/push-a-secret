namespace Domain;

public enum Result
{
    OK,
    Err
}

public record Identifier(string Value);

public record Secret(string Algorithm, string IV, string Ciphertext);

public record IdentifierResult(Result Result, Identifier? Identifier);

public record SecretResult(Result Result, Secret? Secret);
