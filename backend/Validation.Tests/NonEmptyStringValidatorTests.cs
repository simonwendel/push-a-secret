using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Validation.Tests;

public class NonEmptyStringValidatorTests
{
    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(" ")]
    [InlineAutoData(null)]
    public void Validate_GivenNullOrEmptyString_ThrowsException(string value, NonEmptyStringValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }

    [Theory, AutoData]
    public void Validate_GivenStringWithContents_ReturnsValue(string value, NonEmptyStringValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        sut.Validate(untrusted).Should().Be(value);
    }
}
