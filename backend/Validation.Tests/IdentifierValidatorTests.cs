using System;
using AutoFixture.Xunit2;
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
    {
        var untrusted = new UntrustedValue<string>(value);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }
    
    [Theory]
    [InlineAutoData("1y2p0ij32e8e70")]
    public void Validate_GivenTooLongString_ThrowsException(string value, IdentifierValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }
    
    [Theory]
    [InlineAutoData(";")]
    public void Validate_GivenStringWithNotOnlyAsciiLettersOrDigits_ThrowsException(string value, IdentifierValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }
    
    [Theory]
    [InlineAutoData("0")]
    [InlineAutoData("1337")]
    [InlineAutoData("1y2p0ij32e8e7")]
    public void Validate_GivenStringWithContents_ReturnsValue(string value, IdentifierValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        sut.Validate(untrusted).Should().Be(value);
    }
}
