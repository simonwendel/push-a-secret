using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Storage.Tests;

[Collection("Integration")]
public class StorageIntegrationTests
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly Store store;

    public StorageIntegrationTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        var idGenerator = new IdGenerator(new TimestampGenerator(), new Base36Converter());
        var repository = TestMongoDbRepositoryFactory.Build(cleanAll: true);
        store = new Store(repository, idGenerator);
    }

    [Fact]
    public void RoundTrippingWorksAsIntended()
    {
        var original = new CreateRequest(Algorithm: "AES128GCM", IV: "123", Ciphertext: "456");

        var (result, id) = store.Create(original);
        result.Should().Be(Result.OK);
        id.Should().NotBeNull();

        store.Peek(new PeekRequest(id!.Id)).Should().Be(new PeekResponse(Result.OK));

        (result, var secret) = store.Read(new ReadRequest(id.Id));
        result.Should().Be(Result.OK);
        secret.Should().NotBeNull();
        secret.Should().BeEquivalentTo(original);

        store.Delete(new DeleteRequest(id.Id)).Should().Be(new DeleteResponse(Result.OK));
        store.Peek(new PeekRequest(id.Id)).Should().Be(new PeekResponse(Result.Err));

        testOutputHelper.WriteLine("Round-tripped with id = {0}", id.Id);
    }
}
