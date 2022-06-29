using System;
using AutoFixture;
using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Validation;
using Validation.Specific;
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
    internal void Head_GivenInvalidIdentifierString_ReturnsBadRequest()
        => EnsureBadRequestGivenInvalidIdentifier(sut.Head);

    [Fact]
    internal void Head_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound()
        => EnsureForValidIdentifier((untrusted, identifier) =>
        {
            store.Setup(x => x.Peek(identifier)).Returns(Result.Err);
            sut.Head(untrusted).Should().BeAssignableTo<NotFoundResult>();
        });

    [Fact]
    internal void Head_WhenIdentifierDoesHaveDocument_ReturnsOK()
        => EnsureForValidIdentifier((untrusted, identifier) =>
        {
            store.Setup(x => x.Peek(identifier)).Returns(Result.OK);
            sut.Head(untrusted).Should().BeAssignableTo<OkResult>();
        });

    [Fact]
    internal void Get_GivenInvalidIdentifierString_ReturnsBadRequest()
        => EnsureBadRequestGivenInvalidIdentifier(sut.Get);

    [Fact]
    internal void Get_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound()
        => EnsureForValidIdentifier((untrusted, identifier) =>
        {
            store.Setup(x => x.Read(identifier)).Returns(new SecretResult(Result.Err, null));
            sut.Get(untrusted).Should().BeAssignableTo<NotFoundResult>();
        });

    [Theory, AutoData]
    internal void Get_WhenIdentifierDoesHaveValidDocument_ReturnsOK(Secret secret)
        => EnsureForValidIdentifier((untrusted, identifier) =>
        {
            store.Setup(x => x.Read(identifier)).Returns(new SecretResult(Result.OK, secret));
            secretValidator.Setup(x => x.Validate(new UntrustedValue<Secret>(secret))).Returns(secret);
            sut.Get(untrusted).Should().BeAssignableTo<OkObjectResult>().Which.Value.Should().Be(secret);
        });


    [Theory, AutoData]
    internal void Get_WhenIdentifierHasInvalidDocument_ReturnsConflict(Secret secret)
        => EnsureForValidIdentifier((untrusted, identifier) =>
        {
            store.Setup(x => x.Read(identifier)).Returns(new SecretResult(Result.OK, secret));
            secretValidator.Setup(x => x.Validate(new UntrustedValue<Secret>(secret))).Throws<ValidationException>();
            sut.Get(untrusted).Should().BeAssignableTo<ConflictResult>();
        });

    [Theory, AutoData]
    internal void Post_GivenInvalidSecret_ReturnsBadRequest(UntrustedValue<Secret> untrusted)
    {
        secretValidator.Setup(x => x.Validate(untrusted)).Throws<ValidationException>();
        sut.Post(untrusted).Should().BeAssignableTo<BadRequestResult>();
        store.VerifyNoOtherCalls();
    }

    [Theory, AutoData]
    internal void Post_WhenSecretCannotBeSaved_Returns500(UntrustedValue<Secret> untrusted, Secret secret)
    {
        secretValidator.Setup(x => x.Validate(untrusted)).Returns(secret);
        store.Setup(x => x.Create(secret)).Returns(new IdentifierResult(Result.Err, null));
        sut.Post(untrusted).Should().BeAssignableTo<StatusCodeResult>().Which.StatusCode.Should().Be(500);
        VerifyAll();
    }

    [Theory, AutoData]
    internal void Post_WhenSecretWasSaved_ReturnsIdentifier(
        UntrustedValue<Secret> untrusted,
        Secret secret,
        Identifier identifier)
    {
        secretValidator.Setup(x => x.Validate(untrusted)).Returns(secret);
        store.Setup(x => x.Create(secret)).Returns(new IdentifierResult(Result.OK, identifier));
        sut.Post(untrusted).Should().BeAssignableTo<CreatedResult>().Which.Value.Should().Be(secret);
        VerifyAll();
    }

    [Fact]
    internal void Delete_GivenInvalidIdentifierString_ReturnsBadRequest()
        => EnsureBadRequestGivenInvalidIdentifier(sut.Delete);

    [Fact]
    internal void Delete_WhenIdentifierDoesNotHaveDocument_ReturnsNotFound()
        => EnsureForValidIdentifier((untrusted, identifier) =>
        {
            store.Setup(x => x.Delete(identifier)).Returns(Result.Err);
            sut.Delete(untrusted).Should().BeAssignableTo<NotFoundResult>();
        });

    [Fact]
    internal void Delete_WhenIdentifierDoesHaveDocument_ReturnsNoContent()
        => EnsureForValidIdentifier((untrusted, identifier) =>
        {
            store.Setup(x => x.Delete(identifier)).Returns(Result.OK);
            sut.Delete(untrusted).Should().BeAssignableTo<NoContentResult>();
        });

    private void EnsureBadRequestGivenInvalidIdentifier(Func<UntrustedValue<Identifier>, IActionResult> action)
    {
        var untrusted = fixture.Create<UntrustedValue<Identifier>>();
        idValidator.Setup(x => x.Validate(untrusted)).Throws<ValidationException>();
        action(untrusted).Should().BeAssignableTo<BadRequestResult>();
        idValidator.VerifyAll();
        store.VerifyNoOtherCalls();
    }

    private void EnsureForValidIdentifier(Action<UntrustedValue<Identifier>, Identifier> func)
    {
        var (untrusted, identifier) = ConstructCorrectlyValidatingId();
        func(untrusted, identifier);
        VerifyAll();
    }

    private (UntrustedValue<Identifier>, Identifier) ConstructCorrectlyValidatingId()
    {
        var untrusted = fixture.Create<UntrustedValue<Identifier>>();
        var identifier = fixture.Create<Identifier>();
        idValidator.Setup(x => x.Validate(untrusted)).Returns(identifier);
        return (untrusted, identifier);
    }

    private void VerifyAll()
    {
        idValidator.VerifyAll();
        store.VerifyAll();
    }
}
