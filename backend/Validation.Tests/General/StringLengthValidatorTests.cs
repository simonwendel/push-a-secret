using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Validation.General;
using Xunit;

namespace Validation.Tests.General;

public class StringLengthValidatorTests
{
    [Theory]
    [InlineData(10, 9)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    internal void Ctor_GivenInvalidBounds_ThrowsArgumentException(int minLength, int maxLength)
    {
        Action constructing = () => new StringLengthValidator(minLength, maxLength);
        constructing.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(-1, 9)]
    [InlineData(0, -1)]
    internal void Ctor_GivenNegativeBounds_ThrowsArgumentOutOfRangeException(int minLength, int maxLength)
    {
        Action constructing = () => new StringLengthValidator(minLength, maxLength);
        constructing.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineAutoData("", 1)]
    [InlineAutoData("_1_", 4)]
    [InlineAutoData("abc", 6)]
    internal void Validate_GivenTooShortString_ThrowsException(string value, int minLength)
    {
        var untrusted = new UntrustedValue<string>(value);
        var sut = new StringLengthValidator(minLength);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData("12", 0, 1)]
    [InlineAutoData("_1_2_3_4", 2, 4)]
    [InlineAutoData("abcdefghijklmnopqrstuvxyz", 6, 20)]
    internal void Validate_GivenTooLongString_ThrowsException(string value, int minLength, int maxLength)
    {
        var untrusted = new UntrustedValue<string>(value);
        var sut = new StringLengthValidator(minLength, maxLength);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }

    [Fact]
    internal void Validate_GivenStringWithinLengthRange_ReturnsValue()
    {
        var expected = "12345";
        var untrusted = new UntrustedValue<string>(expected);
        var sut = new StringLengthValidator(4, 6);
        sut.Validate(untrusted).Should().Be(expected);
    }
}
