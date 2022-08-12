using System.Runtime.CompilerServices;
using MongoDB.Bson.Serialization.Attributes;

namespace Storage;

internal class SecretEntity
{
    private readonly string? id;
    private readonly string? algorithm;
    private readonly string? iv;
    private readonly string? ciphertext;

    public string Id
    {
        get => GetValidString(id);
        init => id = value;
    }

    public string Algorithm
    {
        get => GetValidString(algorithm);
        init => algorithm = value;
    }

    public string IV
    {
        get => GetValidString(iv);
        init => iv = value;
    }

    public string Ciphertext
    {
        get => GetValidString(ciphertext);
        init => ciphertext = value;
    }

    [BsonElement("_createdAt")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime CreatedAt { get; init; }

    private static string GetValidString(string? value, [CallerArgumentExpression("value")] string field = "")
        => value ?? throw new InvalidOperationException(
            $"{field} can't be null, this entity was not initialized correctly.");
}
