using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Storage;
using Xunit;

namespace Verify.Integration;

[Collection("Verify.Integration")]
public class MongoDbRepositoryIntegrationTests
{
    private readonly MongoDbRepository sut;

    public MongoDbRepositoryIntegrationTests()
        => sut = TestMongoDbRepositoryFactory.Build(cleanAll: true);

    [Fact]
    public void Peek_GivenIdOfDocumentThatDoesNotExist_ReturnsErrorResult()
        => sut.Peek(new Identifier("0")).Should().Be(Result.Err);

    [Fact]
    public void Read_GivenIdOfDocumentThatDoesNotExist_ReturnsErrorResult()
    {
        var (results, secret) = sut.Read(new Identifier("0"));
        results.Should().Be(Result.Err);
        secret.Should().BeNull();
    }

    [Theory, AutoData]
    public void Create_InsertingWithSameIdTwice_ReturnsErrorResult(string id, Secret firstSecret, Secret secondSecret)
    {
        sut.Create(new Identifier(id), firstSecret).Should().Be(Result.OK);
        sut.Create(new Identifier(id), secondSecret).Should().Be(Result.Err);
    }
    
    [Fact]
    public void Delete_GivenIdOfDocumentThatDoesNotExist_ReturnsErrorResult()
        => sut.Delete(new Identifier("0")).Should().Be(Result.Err);
}
