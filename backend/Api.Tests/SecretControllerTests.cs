using System;
using AutoFixture;
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
    private readonly Fixture fixture = new();
    private readonly Mock<IIdentifierValidator> validator = new();
    private readonly Mock<IStore> store = new();
    private readonly SecretController sut;

    public SecretControllerTests()
        => sut = new SecretController(validator.Object, store.Object);

    [Fact]
    public void Head_GivenInvalidIdentifierString_ReturnsBadRequest()
        => EnsureBadRequestGivenInvalidIdentifier(sut.Head);

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
    public void Head_WhenIdentifierDoesHaveDocument_ReturnsOK(UntrustedValue<string> untrusted, string id)
    {
        validator.Setup(x => x.Validate(untrusted)).Returns(id);
        store.Setup(x => x.Peek(new PeekRequest(id))).Returns(new PeekResponse(Result.OK));

        sut.Head(untrusted).Should().BeAssignableTo<OkResult>();

        validator.VerifyAll();
        store.VerifyAll();
    }

    [Fact]
    public void Get_GivenInvalidIdentifierString_ReturnsBadRequest()
        => EnsureBadRequestGivenInvalidIdentifier(sut.Get);

    [Theory, AutoData]
    public void Get_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound(UntrustedValue<string> untrusted, string id)
    {
        validator.Setup(x => x.Validate(untrusted)).Returns(id);
        var request = new ReadRequest(id);
        store.Setup(x => x.Read(request)).Returns(new ReadResponse(Result.Err, null));

        sut.Get(untrusted).Should().BeAssignableTo<NotFoundResult>();

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

    [Fact]
    public void Delete_GivenInvalidIdentifierString_ReturnsBadRequest()
        => EnsureBadRequestGivenInvalidIdentifier(sut.Delete);

    [Theory, AutoData]
    public void Delete_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound(UntrustedValue<string> untrusted, string id)
    {
        validator.Setup(x => x.Validate(untrusted)).Returns(id);
        var request = new DeleteRequest(id);
        store.Setup(x => x.Delete(request)).Returns(new DeleteResponse(Result.Err));

        sut.Delete(untrusted).Should().BeAssignableTo<NotFoundResult>();

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

    private void EnsureBadRequestGivenInvalidIdentifier(Func<UntrustedValue<string>, IActionResult> action)
    {
        var untrusted = fixture.Create<UntrustedValue<string>>();
        validator.Setup(x => x.Validate(untrusted)).Throws<ValidationException>();
        action(untrusted).Should().BeAssignableTo<BadRequestResult>();
        validator.VerifyAll();
        store.VerifyNoOtherCalls();
    }
}
