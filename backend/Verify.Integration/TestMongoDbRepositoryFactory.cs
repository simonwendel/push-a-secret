using Storage;

namespace Verify.Integration;

internal class TestMongoDbRepositoryFactory : MongoDbRepositoryFactory
{
    private static readonly StorageConfiguration config = new()
    {
        ConnectionString = "mongodb://localhost:27017",
        DatabaseName = "push-a-secret-tests",
        CollectionName = "test-secrets"
    };

    public TestMongoDbRepositoryFactory()
        : base(config)
    {
    }

    public new MongoDbRepository Build()
        => Build(cleanAll: true);
}
