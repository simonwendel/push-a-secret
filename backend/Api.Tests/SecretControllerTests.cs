using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Storage;
using Validation;
using Xunit;

namespace Api.Tests;

public class SecretControllerTests
{
    private readonly Mock<IIdentifierValidator> validator = new();
    private readonly Mock<IStore> store = new();
    private readonly SecretController sut;

    public SecretControllerTests()
    {
        sut = new SecretController(validator.Object, store.Object);
    }

    [Theory, AutoData]
    public void Head_GivenInvalidIdentifierString_ReturnsBadRequest(UntrustedValue<string> untrusted)
    {
        validator.Setup(x => x.Validate(untrusted)).Throws<ValidationException>();
        sut.Head(untrusted).Should().BeAssignableTo<BadRequestObjectResult>();
        validator.VerifyAll();
        store.VerifyNoOtherCalls();
    }

    [Theory, AutoData]
    public void Head_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound(UntrustedValue<string> untrusted, string id)
    {
        validator.Setup(x => x.Validate(untrusted)).Returns(id);
        var request = new PeekRequest(id);
        store.Setup(x => x.Peek(request)).Returns(new PeekResponse(Result.Err));

        sut.Head(untrusted).Should().BeAssignableTo<NotFoundResult>();

        validator.VerifyAll();
        store.VerifyAll();
    }

    [Theory, AutoData]
    public void Head_WhenIdentifierDoesHaveDocument_ReturnsOK(UntrustedValue<string> untrusted, string id,
        Secret secret)
    {
        validator.Setup(x => x.Validate(untrusted)).Returns(id);
        store.Setup(x => x.Peek(new PeekRequest(id))).Returns(new PeekResponse(Result.OK));

        sut.Head(untrusted).Should().BeAssignableTo<OkResult>();

        validator.VerifyAll();
        store.VerifyAll();
    }

    [Theory, AutoData]
    public void Get_GivenInvalidIdentifierString_ReturnsBadRequest(UntrustedValue<string> untrusted)
    {
        validator.Setup(x => x.Validate(untrusted)).Throws<ValidationException>();
        sut.Get(untrusted).Should().BeAssignableTo<BadRequestObjectResult>();
        validator.VerifyAll();
        store.VerifyNoOtherCalls();
    }

    [Theory, AutoData]
    public void Get_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound(UntrustedValue<string> untrusted, string id)
    {
        validator.Setup(x => x.Validate(untrusted)).Returns(id);
        var request = new ReadRequest(id);
        store.Setup(x => x.Read(request)).Returns(new ReadResponse(Result.Err, null));

        sut.Get(untrusted).Should().BeAssignableTo<NotFoundObjectResult>().Which.Value.Should().Be(request);

        validator.VerifyAll();
        store.VerifyAll();
    }

    [Theory, AutoData]
    public void Get_WhenIdentifierDoesHaveDocument_ReturnsOK(UntrustedValue<string> untrusted, string id, Secret secret)
    {
        validator.Setup(x => x.Validate(untrusted)).Returns(id);
        store.Setup(x => x.Read(new ReadRequest(id))).Returns(new ReadResponse(Result.OK, secret));

        sut.Get(untrusted).Should().BeAssignableTo<OkObjectResult>().Which.Value.Should().Be(secret);

        validator.VerifyAll();
        store.VerifyAll();
    }

    [Theory, AutoData]
    public void Delete_GivenInvalidIdentifierString_ReturnsBadRequest(UntrustedValue<string> untrusted)
    {
        validator.Setup(x => x.Validate(untrusted)).Throws<ValidationException>();
        sut.Delete(untrusted).Should().BeAssignableTo<BadRequestObjectResult>();
        validator.VerifyAll();
        store.VerifyNoOtherCalls();
    }

    [Theory, AutoData]
    public void Delete_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound(UntrustedValue<string> untrusted, string id)
    {
        validator.Setup(x => x.Validate(untrusted)).Returns(id);
        var request = new DeleteRequest(id);
        store.Setup(x => x.Delete(request)).Returns(new DeleteResponse(Result.Err));

        sut.Delete(untrusted).Should().BeAssignableTo<NotFoundObjectResult>().Which.Value.Should().Be(request);

        validator.VerifyAll();
        store.VerifyAll();
    }

    [Theory, AutoData]
    public void Delete_WhenIdentifierDoesHaveDocument_ReturnsNoContent(UntrustedValue<string> untrusted, string id)
    {
        validator.Setup(x => x.Validate(untrusted)).Returns(id);
        store.Setup(x => x.Delete(new DeleteRequest(id))).Returns(new DeleteResponse(Result.OK));

        sut.Delete(untrusted).Should().BeAssignableTo<NoContentResult>();

        validator.VerifyAll();
        store.VerifyAll();
    }
}
