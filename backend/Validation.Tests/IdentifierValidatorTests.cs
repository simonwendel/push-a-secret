using System;
using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Xunit;

namespace Validation.Tests;

public class IdentifierValidatorTests
{
    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(" ")]
    [InlineAutoData(null)]
    public void Validate_GivenNullOrEmptyString_ThrowsException(string value, IdentifierValidator sut)
        => EnsureForUntrustedIdentifier(value, untrusted =>
        {
            Action validating = () => sut.Validate(untrusted);
            validating.Should().Throw<ValidationException>();
        });

    [Theory]
    [InlineAutoData("1y2p0ij32e8e70")]
    public void Validate_GivenTooLongString_ThrowsException(string value, IdentifierValidator sut)
        => EnsureForUntrustedIdentifier(value, untrusted =>
        {
            Action validating = () => sut.Validate(untrusted);
            validating.Should().Throw<ValidationException>();
        });

    [Theory]
    [InlineAutoData(";")]
    public void Validate_GivenStringWithNonAsciiLettersOrDigits_ThrowsException(string value, IdentifierValidator sut)
        => EnsureForUntrustedIdentifier(value, untrusted =>
        {
            Action validating = () => sut.Validate(untrusted);
            validating.Should().Throw<ValidationException>();
        });

    [Theory]
    [InlineAutoData("0")]
    [InlineAutoData("1337")]
    [InlineAutoData("1y2p0ij32e8e7")]
    public void Validate_GivenStringWithContents_ReturnsValue(string value, IdentifierValidator sut)
        => EnsureForUntrustedIdentifier(
            value, untrusted =>
            {
                var expected = new Identifier(value);
                sut.Validate(untrusted).Should().Be(expected);
            });

    private static void EnsureForUntrustedIdentifier(string value, Action<UntrustedValue<Identifier>> request)
    {
        var identifier = new Identifier(value);
        var untrusted = new UntrustedValue<Identifier>(identifier);
        request(untrusted);
    }
}
