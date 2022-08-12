using MongoDB.Driver;

namespace Storage;

internal static class MongoDbRepositoryFactory
{
    private const string ConnectionString = "mongodb://localhost:27017";
    private const string DatabaseName = "push-a-secret";
    private const string CollectionName = "secrets";

    public static MongoDbRepository Build()
        => Build(ConnectionString, DatabaseName, CollectionName, TimeSpan.FromSeconds(60), cleanAll: false);
    
    public static MongoDbRepository Build(
        string connectionString,
        string databaseName,
        string collectionName,
        TimeSpan expiry,
        bool cleanAll = false)
    {
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(databaseName);
        if (cleanAll)
        {
            database.DropCollection(collectionName);
        }
        
        var collection = database.GetCollection<SecretEntity>(collectionName);

        if (collection.Indexes.List().ToList().Any(x => x.GetValue("name").AsString == "expireAfterSecondsIndex"))
            return new MongoDbRepository(collection);

        var keys = Builders<SecretEntity>.IndexKeys.Ascending("_createdAt");
        var options = new CreateIndexOptions
        {
            Name = "expireAfterSecondsIndex",
            ExpireAfter = expiry
        };

        var indexModel = new CreateIndexModel<SecretEntity>(keys, options);
        collection.Indexes.CreateOne(indexModel);

        return new MongoDbRepository(collection);
    }
}
