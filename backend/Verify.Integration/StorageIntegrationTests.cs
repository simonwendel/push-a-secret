using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Xunit;

namespace Verify.Integration;

[Collection("Verify.Integration")]
public class StorageIntegrationTests
{
    private Store? store;

    public StorageIntegrationTests() 
        => InitializeTestFixture();

    [Theory, AutoData]
    public void RoundTrip_WhenRunAgainstMongoDb_WorksAsIntended(CreateRequest original)
        => store
            .InsertNewSecret(original)
            .VerifyItExists()
            .VerifyItCanBeRead()
            .DeleteTheSecret()
            .VerifyItIsGone();
    
    private void InitializeTestFixture()
    {
        var idGenerator = IdGenerator.Default;
        var repository = TestMongoDbRepositoryFactory.Build(cleanAll: true);
        store = new Store(repository, idGenerator);
    }
}

internal static class StorageIntegrationTestExtensions
{
    public static (IStore, Identifier, CreateRequest) InsertNewSecret(this IStore? store, CreateRequest original)
    {
        var (result, identifier) = store!.Create(original);
        result.Should().Be(Result.OK);
        identifier.Should().NotBeNull();
        return (store, identifier, original)!;
    }

    public static (IStore, Identifier, CreateRequest) VerifyItExists(this (IStore, Identifier, CreateRequest) chain)
    {
        var (store, identifier, _) = chain;
        var okPeek = new PeekResponse(Result.OK);
        store.Peek(new PeekRequest(identifier.Id)).Should().Be(okPeek);
        return chain;
    }

    public static (IStore, Identifier, CreateRequest) VerifyItCanBeRead(
        this (IStore, Identifier, CreateRequest) chain)
    {
        var (store, identifier, original) = chain;
        var (result, secret) = store.Read(new ReadRequest(identifier.Id));
        result.Should().Be(Result.OK);
        secret.Should().NotBeNull();
        secret.Should().BeEquivalentTo(original);
        return chain;
    }

    public static (IStore, Identifier, CreateRequest) DeleteTheSecret(this (IStore, Identifier, CreateRequest) chain)
    {
        var (store, identifier, _) = chain;
        var okDelete = new DeleteResponse(Result.OK);
        store.Delete(new DeleteRequest(identifier.Id)).Should().Be(okDelete);
        return chain;
    }

    public static void VerifyItIsGone(this (IStore, Identifier, CreateRequest) chain)
    {
        var (store, identifier, _) = chain;
        var errorPeek = new PeekResponse(Result.Err);
        store.Peek(new PeekRequest(identifier.Id)).Should().Be(errorPeek);
    }
}
