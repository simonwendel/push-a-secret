using System;
using Storage;

namespace Verify.Integration;

internal class TestMongoDbRepositoryFactory : MongoDbRepositoryFactory
{
    private const string ConnectionString = "mongodb://localhost:27017";
    private const string TestDatabaseName = "push-a-secret-tests";
    private const string TestCollectionName = "test-secrets";

    public new static MongoDbRepository Build()
        => Build(
            ConnectionString, 
            TestDatabaseName, 
            TestCollectionName,
            cleanAll: true);
}
