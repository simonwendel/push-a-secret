using MongoDB.Driver;

namespace Storage;

internal class MongoDbRepositoryFactory : IMongoDbRepositoryFactory
{
    private const string expiryIndexName = "expireAtIndex";
    private const string expiryFieldName = "_expireAt";
    private readonly StorageConfiguration config;

    public MongoDbRepositoryFactory(StorageConfiguration config)
        => this.config = config;

    public MongoDbRepository Build()
        => Build(dropCollection: false);

    protected MongoDbRepository Build(bool dropCollection)
    {
        MakeSureConfigIsValid(config);

        var database = ConnectToDatabase(
            config.ConnectionString!,
            config.DatabaseName!);

        var collection = GetCollection(
            database,
            config.CollectionName!,
            dropCollection);

        if (!ExpiryIndexExists(collection))
        {
            CreateExpiryIndex(collection);
        }

        return new MongoDbRepository(collection);
    }

    private static void MakeSureConfigIsValid(StorageConfiguration config)
    {
        if (!config.IsValid())
        {
            throw new InvalidOperationException(
                "Illegal StorageConfiguration, are the appsettings.{*}.json files correct?");
        }
    }

    private static IMongoCollection<SecretEntity> GetCollection(
        IMongoDatabase database,
        string collectionName,
        bool dropCollection)
    {
        if (dropCollection)
        {
            database.DropCollection(collectionName);
        }

        return database.GetCollection<SecretEntity>(collectionName);
    }

    private static IMongoDatabase ConnectToDatabase(string connectionString, string databaseName)
    {
        var mongoClient = new MongoClient(connectionString);
        return mongoClient.GetDatabase(databaseName);
    }

    private static bool ExpiryIndexExists(IMongoCollection<SecretEntity> collection)
        => collection.Indexes.List().ToList()
            .Any(x => x.GetValue("name").AsString.Equals(expiryIndexName));

    private static void CreateExpiryIndex(IMongoCollection<SecretEntity> collection)
    {
        var keys = Builders<SecretEntity>.IndexKeys.Ascending(expiryFieldName);
        var options = new CreateIndexOptions
        {
            Name = expiryIndexName,
            ExpireAfter = TimeSpan.FromSeconds(0)
        };

        var indexModel = new CreateIndexModel<SecretEntity>(keys, options);
        collection.Indexes.CreateOne(indexModel);
    }
}
