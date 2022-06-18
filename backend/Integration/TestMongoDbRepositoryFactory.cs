using MongoDB.Bson;
using MongoDB.Driver;
using Storage;

namespace Integration;

internal static class TestMongoDbRepositoryFactory
{
    private const string ConnectionString = "mongodb://localhost:27017";
    private const string TestDatabaseName = "push-a-secret-tests";
    private const string TestCollectionName = "test-secrets";

    public static MongoDbRepository Build(bool cleanAll = true)
    {
        var mongoClient = new MongoClient(ConnectionString);
        var database = mongoClient.GetDatabase(TestDatabaseName);
        if (cleanAll)
        {
            database.DropCollection(TestCollectionName);
        }

        var collection = database.GetCollection<BsonDocument>(TestCollectionName);
        return new MongoDbRepository(collection);
    }
}
