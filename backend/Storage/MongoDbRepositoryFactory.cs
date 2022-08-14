using MongoDB.Driver;

namespace Storage;

internal class MongoDbRepositoryFactory : IMongoDbRepositoryFactory
{
    private const string ConnectionString = "mongodb://localhost:27017";
    private const string DatabaseName = "push-a-secret";
    private const string CollectionName = "secrets";

    public MongoDbRepository Build()
        => Build(ConnectionString, DatabaseName, CollectionName, cleanAll: false);

    protected static MongoDbRepository Build(
        string connectionString,
        string databaseName,
        string collectionName,
        bool cleanAll = false)
    {
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(databaseName);
        if (cleanAll)
        {
            database.DropCollection(collectionName);
        }
        
        var collection = database.GetCollection<SecretEntity>(collectionName);

        if (collection.Indexes.List().ToList().Any(x => x.GetValue("name").AsString == "expireAtIndex"))
            return new MongoDbRepository(collection);

        var keys = Builders<SecretEntity>.IndexKeys.Ascending("_expireAt");
        var options = new CreateIndexOptions
        {
            Name = "expireAtIndex",
            ExpireAfter = TimeSpan.FromSeconds(0)
        };

        var indexModel = new CreateIndexModel<SecretEntity>(keys, options);
        collection.Indexes.CreateOne(indexModel);

        return new MongoDbRepository(collection);
    }
}
