using System;
using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Moq;
using Validation.Specific;
using Xunit;

namespace Validation.Tests;

public class ValidatorTests
{
    private readonly Mock<IIdentifierValidator> idValidator = new();
    private readonly Mock<ISecretValidator> secretValidator = new();
    private readonly Validator sut;

    public ValidatorTests()
        => sut = new Validator(idValidator.Object, secretValidator.Object);

    [Theory, AutoData]
    public void Validate_GivenIdentifier_UsesIdentifierValidator(UntrustedValue<Identifier> untrusted, Identifier validated)
    {
        idValidator.Setup(x => x.Validate(untrusted)).Returns(validated);
        sut.Validate(untrusted).Should().Be(validated);
        idValidator.VerifyAll();
    }

    [Theory, AutoData]
    public void Validate_GivenSecret_UsesSecretValidator(UntrustedValue<Secret> untrusted, Secret validated)
    {
        
        secretValidator.Setup(x => x.Validate(untrusted)).Returns(validated);
        sut.Validate(untrusted).Should().Be(validated);
        secretValidator.VerifyAll();
    }

    [Theory, AutoData]
    public void Validate_GivenInvalidType_ThrowsException(UntrustedValue<object> untrusted)
    {
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<InvalidOperationException>();
    }
}
