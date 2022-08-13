using System.Runtime.CompilerServices;
using MongoDB.Bson.Serialization.Attributes;

namespace Storage;

internal class SecretEntity
{
    private readonly string? id;
    private readonly string? algorithm;
    private readonly string? iv;
    private readonly string? ciphertext;
    private readonly int? ttl;

    public string Id
    {
        get => GetValidField(id);
        init => id = value;
    }

    public string Algorithm
    {
        get => GetValidField(algorithm);
        init => algorithm = value;
    }

    public string IV
    {
        get => GetValidField(iv);
        init => iv = value;
    }

    public string Ciphertext
    {
        get => GetValidField(ciphertext);
        init => ciphertext = value;
    }

    public int Ttl
    {
        get => (int)GetValidField(ttl)!;
        init => ttl = value;
    }

    [BsonElement("_createdAt")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime CreatedAt { get; init; }

    private static T GetValidField<T>(T? value, [CallerArgumentExpression("value")] string field = "")
        => value ?? throw new InvalidOperationException(
            $"{field} can't be null, this entity was not initialized correctly.");
}
