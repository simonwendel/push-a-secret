using MongoDB.Driver;

namespace Storage;

internal static class MongoDbRepositoryFactory
{
    private const string ConnectionString = "mongodb://localhost:27017";
    private const string DatabaseName = "push-a-secret";
    private const string CollectionName = "secrets";

    public static MongoDbRepository Build()
    {
        var mongoClient = new MongoClient(ConnectionString);
        var database = mongoClient.GetDatabase(DatabaseName);
        var collection = database.GetCollection<SecretEntity>(CollectionName);
        return new MongoDbRepository(collection);
    }
}
