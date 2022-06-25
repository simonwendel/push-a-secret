using System;
using AutoFixture;
using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Validation;
using Xunit;

namespace Api.Tests;

public class SecretControllerTests
{
    private readonly Fixture fixture = new();
    private readonly Mock<IIdentifierValidator> idValidator = new();
    private readonly Mock<ISecretValidator> secretValidator = new();
    private readonly Mock<IStore> store = new();
    private readonly SecretController sut;

    public SecretControllerTests()
        => sut = new SecretController(
            store.Object,
            idValidator.Object,
            secretValidator.Object);

    [Fact]
    public void Head_GivenInvalidIdentifierString_ReturnsBadRequest()
        => EnsureBadRequestGivenInvalidIdentifier(sut.Head);

    [Fact]
    public void Head_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound()
        => EnsureForValidIdentifier((untrusted, id) =>
        {
            store.Setup(x => x.Peek(new PeekRequest(id))).Returns(new PeekResponse(Result.Err));
            sut.Head(untrusted).Should().BeAssignableTo<NotFoundResult>();
        });

    [Fact]
    public void Head_WhenIdentifierDoesHaveDocument_ReturnsOK()
        => EnsureForValidIdentifier((untrusted, id) =>
        {
            store.Setup(x => x.Peek(new PeekRequest(id))).Returns(new PeekResponse(Result.OK));
            sut.Head(untrusted).Should().BeAssignableTo<OkResult>();
        });

    [Fact]
    public void Get_GivenInvalidIdentifierString_ReturnsBadRequest()
        => EnsureBadRequestGivenInvalidIdentifier(sut.Get);

    [Fact]
    public void Get_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound()
    {
        var (untrusted, id) = ConstructCorrectlyValidatingId();
        store.Setup(x => x.Read(new ReadRequest(id))).Returns(new ReadResponse(Result.Err, null));
        sut.Get(untrusted).Should().BeAssignableTo<NotFoundResult>();
        VerifyAll();
    }

    [Theory, AutoData]
    public void Get_WhenIdentifierDoesHaveValidDocument_ReturnsOK(Secret secret)
    {
        var (untrusted, id) = ConstructCorrectlyValidatingId();
        store.Setup(x => x.Read(new ReadRequest(id))).Returns(new ReadResponse(Result.OK, secret));
        secretValidator.Setup(x => x.Validate(new UntrustedValue<Secret>(secret))).Returns(secret);
        sut.Get(untrusted).Should().BeAssignableTo<OkObjectResult>().Which.Value.Should().Be(secret);
        VerifyAll();
    }

    [Theory, AutoData]
    internal void Get_WhenIdentifierHasInvalidDocument_ReturnsConflict(Secret secret)
    {
        var (untrusted, id) = ConstructCorrectlyValidatingId();
        store.Setup(x => x.Read(new ReadRequest(id))).Returns(new ReadResponse(Result.OK, secret));
        secretValidator.Setup(x => x.Validate(new UntrustedValue<Secret>(secret))).Throws<ValidationException>();
        sut.Get(untrusted).Should().BeAssignableTo<ConflictResult>();
        VerifyAll();
    }

    [Fact]
    public void Delete_GivenInvalidIdentifierString_ReturnsBadRequest()
        => EnsureBadRequestGivenInvalidIdentifier(sut.Delete);

    [Fact]
    public void Delete_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound()
        => EnsureForValidIdentifier((untrusted, id) =>
        {
            store.Setup(x => x.Delete(new DeleteRequest(id))).Returns(new DeleteResponse(Result.Err));
            sut.Delete(untrusted).Should().BeAssignableTo<NotFoundResult>();
        });

    [Fact]
    public void Delete_WhenIdentifierDoesHaveDocument_ReturnsNoContent()
        => EnsureForValidIdentifier((untrusted, id) =>
        {
            store.Setup(x => x.Delete(new DeleteRequest(id))).Returns(new DeleteResponse(Result.OK));
            sut.Delete(untrusted).Should().BeAssignableTo<NoContentResult>();
        });

    private void EnsureBadRequestGivenInvalidIdentifier(Func<UntrustedValue<string>, IActionResult> action)
    {
        var untrusted = fixture.Create<UntrustedValue<string>>();
        idValidator.Setup(x => x.Validate(untrusted)).Throws<ValidationException>();
        action(untrusted).Should().BeAssignableTo<BadRequestResult>();
        idValidator.VerifyAll();
        store.VerifyNoOtherCalls();
    }

    private void EnsureForValidIdentifier(Action<UntrustedValue<string>, string> func)
    {
        var (untrusted, id) = ConstructCorrectlyValidatingId();
        func(untrusted, id);
        VerifyAll();
    }

    private (UntrustedValue<string>, string) ConstructCorrectlyValidatingId()
    {
        var untrusted = fixture.Create<UntrustedValue<string>>();
        var id = fixture.Create<string>();
        idValidator.Setup(x => x.Validate(untrusted)).Returns(id);
        return (untrusted, id);
    }

    private void VerifyAll()
    {
        idValidator.VerifyAll();
        store.VerifyAll();
    }
}
