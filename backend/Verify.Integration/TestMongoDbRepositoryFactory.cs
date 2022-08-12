using System;
using Storage;

namespace Verify.Integration;

internal static class TestMongoDbRepositoryFactory
{
    private const string ConnectionString = "mongodb://localhost:27017";
    private const string TestDatabaseName = "push-a-secret-tests";
    private const string TestCollectionName = "test-secrets";

    public static MongoDbRepository Build(bool cleanAll = false)
        => MongoDbRepositoryFactory.Build(
            ConnectionString, 
            TestDatabaseName, 
            TestCollectionName,
            expiry: TimeSpan.FromMinutes(5), 
            cleanAll: true);
}
