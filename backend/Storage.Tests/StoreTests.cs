using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Xunit;

namespace Storage.Tests;

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
    public void Peek_GivenRequest_PeeksAndReturnsResult(Result result, PeekRequest request)
    {
        repository.Setup(x => x.Peek(request)).Returns(result);
        var expected = new PeekResponse(result);
        sut.Peek(request).Should().Be(expected);
        repository.VerifyAll();
    }

    [Theory, AutoData]
    public void Create_WhenSecretCouldBeStored_StoresAndReturnsOKResult(
        Identifier id,
        CreateRequest request)
    {
        idGenerator.Setup(x => x.Generate()).Returns(id);
        repository.Setup(x => x.Create(id, request)).Returns(Result.OK);

        var expected = new CreateResponse(Result.OK, id);
        sut.Create(request).Should().Be(expected);

        idGenerator.VerifyAll();
        repository.VerifyAll();
    }

    [Theory, AutoData]
    public void Create_WhenSecretCouldNotBeStored_ReturnsErrorResult(Identifier id, CreateRequest request)
    {
        idGenerator.Setup(x => x.Generate()).Returns(id);
        repository.Setup(x => x.Create(id, request)).Returns(Result.Err);

        var expected = new CreateResponse(Result.Err, null);
        sut.Create(request).Should().Be(expected);

        idGenerator.VerifyAll();
        repository.VerifyAll();
    }

    [Theory, AutoData]
    public void Read_WhenSecretCouldBeRead_ReadsAndReturnsOKResult(Secret secret, ReadRequest request)
    {
        var read = (Result.OK, secret);
        repository.Setup(x => x.Read(request)).Returns(read);

        var expected = new ReadResponse(Result.OK, secret);
        sut.Read(request).Should().Be(expected);

        repository.VerifyAll();
    }

    [Theory, AutoData]
    public void Read_WhenSecretCouldNotBeRead_ReturnsErrorResult(ReadRequest request)
    {
        (Result, Secret) read = (Result.Err, null)!;
        repository.Setup(x => x.Read(request)).Returns(read);

        var expected = new ReadResponse(Result.Err, null);
        sut.Read(request).Should().Be(expected);

        repository.VerifyAll();
    }

    [Theory, AutoData]
    public void Read_WhenReadFromRepositoryFails_DiscardsSecretEvenIfReturned(Secret secret, ReadRequest request)
    {
        var read = (Result.Err, secret);
        repository.Setup(x => x.Read(request)).Returns(read);

        var expected = new ReadResponse(Result.Err, null);
        sut.Read(request).Should().Be(expected);

        repository.VerifyAll();
    }

    [Theory]
    [InlineAutoData(Result.OK)]
    [InlineAutoData(Result.Err)]
    public void Delete_GivenRequest_RespondsWithRepositoryResults(Result result, DeleteRequest request)
    {
        repository.Setup(x => x.Delete(request)).Returns(result);
        var expected = new DeleteResponse(result);
        sut.Delete(request).Should().Be(expected);
        repository.VerifyAll();
    }
}
