using MongoDB.Driver;

namespace Storage;

internal class MongoDbRepositoryFactory : IMongoDbRepositoryFactory
{
    private readonly StorageConfiguration config;

    public MongoDbRepositoryFactory(StorageConfiguration config)
    {
        this.config = config;
    }

    public MongoDbRepository Build()
        => Build(cleanAll: false);

    protected MongoDbRepository Build(
        bool cleanAll)
    {
        var mongoClient = new MongoClient(config.ConnectionString);
        var database = mongoClient.GetDatabase(config.DatabaseName);
        if (cleanAll)
        {
            database.DropCollection(config.CollectionName);
        }
        
        var collection = database.GetCollection<SecretEntity>(config.CollectionName);

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
