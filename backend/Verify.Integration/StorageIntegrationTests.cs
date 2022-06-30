using System;
using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Xunit;

namespace Verify.Integration;

[Collection("Verify.Integration")]
public class StorageIntegrationTests
{
    private readonly Store store;

    public StorageIntegrationTests() 
        => store = InitializeTestFixture();

    [Theory, AutoData]
    internal void RoundTrip_WhenRunAgainstMongoDb_WorksAsIntended(Secret original)
        => store
            .InsertNewSecret(original)
            .VerifyItExists()
            .VerifyItCanBeRead()
            .DeleteTheSecret()
            .VerifyItIsGone();
    
    private Store InitializeTestFixture()
    {
        var idGenerator = IdGenerator.Default;
        var repository = TestMongoDbRepositoryFactory.Build(cleanAll: true);
        return new Store(repository, idGenerator);
    }
}

internal static class StorageIntegrationTestExtensions
{
    public static (IStore, Identifier, Secret) InsertNewSecret(this IStore store, Secret original)
    {
        var (result, identifier) = store.Create(original);
        if (identifier is null)
        {
            throw new InvalidOperationException();
        }
        
        result.Should().Be(Result.OK);
        identifier.Should().NotBeNull();
        return (store, identifier, original);
    }

    public static (IStore, Identifier, Secret) VerifyItExists(this (IStore, Identifier, Secret) chain)
    {
        var (store, identifier, _) = chain;
        store.Peek(identifier).Should().Be(Result.OK);
        return chain;
    }

    public static (IStore, Identifier, Secret) VerifyItCanBeRead(
        this (IStore, Identifier, Secret) chain)
    {
        var (store, identifier, original) = chain;
        var (result, secret) = store.Read(identifier);
        result.Should().Be(Result.OK);
        secret.Should().NotBeNull();
        secret.Should().BeEquivalentTo(original);
        return chain;
    }

    public static (IStore, Identifier, Secret) DeleteTheSecret(this (IStore, Identifier, Secret) chain)
    {
        var (store, identifier, _) = chain;
        store.Delete(identifier).Should().Be(Result.OK);
        return chain;
    }

    public static void VerifyItIsGone(this (IStore, Identifier, Secret) chain)
    {
        var (store, identifier, _) = chain;
        store.Peek(identifier).Should().Be(Result.Err);
    }
}
