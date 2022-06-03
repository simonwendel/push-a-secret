using MongoDB.Bson;
using MongoDB.Driver;

namespace Storage;

internal class MongoDbRepository : IRepository
{
    private readonly IMongoCollection<BsonDocument> collection;

    public MongoDbRepository(IMongoCollection<BsonDocument> collection) 
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
        
        var document = new BsonDocument
        {
            {"id", id.Id},
            {"algorithm", secret.Algorithm},
            {"iv", secret.IV},
            {"ciphertext", secret.Ciphertext}
        };

        try
        {
            collection.InsertOne(document);
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
            var document = collection.Find(FilterId(id)).Single();
            var secret = new Secret(
                document.GetValue("algorithm").AsString,
                document.GetValue("iv").AsString,
                document.GetValue("ciphertext").AsString);
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

    private static FilterDefinition<BsonDocument> FilterId(Identifier id)
        => Builders<BsonDocument>.Filter.Eq("id", id.Id);
}
