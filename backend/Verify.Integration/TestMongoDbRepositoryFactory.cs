// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

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
        => Build(dropCollection: true);
}
