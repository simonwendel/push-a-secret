using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Xunit;

namespace Domain.Tests;

public class StoreTests
{
    private readonly Mock<IIdGenerator> idGenerator = new();
    private readonly Mock<IRepository> repository = new();
    private readonly Store sut;

    public StoreTests()
        => sut = new Store(repository.Object, idGenerator.Object);

    [Theory]
    [InlineAutoData(Result.OK)]
    [InlineAutoData(Result.Err)]
    internal void Peek_GivenIdentifier_PeeksAndReturnsResult(Result result, Identifier identifier)
    {
        repository.Setup(x => x.Peek(identifier)).Returns(result);
        sut.Peek(identifier).Should().Be(result);
        repository.VerifyAll();
    }

    [Theory, AutoData]
    internal void Create_WhenSecretCouldBeStored_StoresAndReturnsOKResult(Identifier identifier, Secret secret)
    {
        idGenerator.Setup(x => x.Generate()).Returns(identifier);
        repository.Setup(x => x.Create(identifier, secret)).Returns(Result.OK);

        var expected = new IdentifierResult(Result.OK, identifier);
        sut.Create(secret).Should().Be(expected);

        idGenerator.VerifyAll();
        repository.VerifyAll();
    }

    [Theory, AutoData]
    internal void Create_WhenSecretCouldNotBeStored_ReturnsErrorResult(Identifier id, Secret secret)
    {
        idGenerator.Setup(x => x.Generate()).Returns(id);
        repository.Setup(x => x.Create(id, secret)).Returns(Result.Err);

        var expected = new IdentifierResult(Result.Err, null);
        sut.Create(secret).Should().Be(expected);

        idGenerator.VerifyAll();
        repository.VerifyAll();
    }

    [Theory, AutoData]
    internal void Read_WhenSecretCouldBeRead_ReadsAndReturnsOKResult(Secret secret, Identifier identifier)
    {
        var read = (Result.OK, secret);
        repository.Setup(x => x.Read(identifier)).Returns(read);

        var expected = new SecretResult(Result.OK, secret);
        sut.Read(identifier).Should().Be(expected);

        repository.VerifyAll();
    }

    [Theory, AutoData]
    internal void Read_WhenSecretCouldNotBeRead_ReturnsErrorResult(Identifier identifier)
    {
        (Result, Secret) read = (Result.Err, null)!;
        repository.Setup(x => x.Read(identifier)).Returns(read);

        var expected = new SecretResult(Result.Err, null);
        sut.Read(identifier).Should().Be(expected);

        repository.VerifyAll();
    }

    [Theory, AutoData]
    internal void Read_WhenReadFromRepositoryFails_DiscardsSecretEvenIfReturned(Secret secret, Identifier identifier)
    {
        var read = (Result.Err, secret);
        repository.Setup(x => x.Read(identifier)).Returns(read);

        var expected = new SecretResult(Result.Err, null);
        sut.Read(identifier).Should().Be(expected);

        repository.VerifyAll();
    }

    [Theory]
    [InlineAutoData(Result.OK)]
    [InlineAutoData(Result.Err)]
    internal void Delete_GivenIdentifier_RespondsWithRepositoryResults(Result result, Identifier identifier)
    {
        repository.Setup(x => x.Delete(identifier)).Returns(result);
        sut.Delete(identifier).Should().Be(result);
        repository.VerifyAll();
    }
}
