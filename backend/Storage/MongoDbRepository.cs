using MongoDB.Driver;
using Domain;

namespace Storage;

internal class MongoDbRepository : IRepository
{
    private readonly IMongoCollection<SecretEntity> collection;

    public MongoDbRepository(IMongoCollection<SecretEntity> collection)
        => this.collection = collection;

    public Result Peek(Identifier id)
    {
        var count = collection.CountDocuments(FilterId(id));
        return count > 0 ? Result.OK : Result.Err;
    }

    public Result Create(Identifier id, Secret secret)
    {
        if (Peek(id) == Result.OK)
        {
            return Result.Err;
        }

        var entity = new SecretEntity
        {
            Id = id.Value,
            Algorithm = secret.Algorithm,
            IV = secret.IV,
            Ciphertext = secret.Ciphertext,
            Ttl = secret.Ttl,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            collection.InsertOne(entity);
            return Result.OK;
        }
        catch (Exception)
        {
            return Result.Err;
        }
    }

    public (Result result, Secret? secret) Read(Identifier id)
    {
        try
        {
            var entity = collection.Find(FilterId(id)).Single();
            var secret = new Secret(
                entity.Algorithm,
                entity.IV,
                entity.Ciphertext,
                entity.Ttl);

            return (Result.OK, secret);
        }
        catch (Exception)
        {
            return (Result.Err, null);
        }
    }

    public Result Delete(Identifier id)
    {
        try
        {
            var deleted = collection.DeleteMany(FilterId(id)).DeletedCount;
            return deleted > 0 ? Result.OK : Result.Err;
        }
        catch (Exception)
        {
            return Result.Err;
        }
    }

    private static FilterDefinition<SecretEntity> FilterId(Identifier id)
        => Builders<SecretEntity>.Filter.Eq(x => x.Id, id.Value);
}
