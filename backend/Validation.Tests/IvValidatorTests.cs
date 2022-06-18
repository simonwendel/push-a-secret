using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Validation.Tests;

public class IvValidatorTests
{
    [Fact]
    public void IvLength_AsPerConfiguration_ShouldBe16()
        => IvValidator.IvLength.Should().Be(16);

    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(" ")]
    [InlineAutoData("\n")]
    [InlineAutoData("\t")]
    public void Validate_GivenEmptyString_ThrowsException(string value, IvValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(value));
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData("qqqqqqqqqqqqqqqqq")]
    [InlineAutoData("qqqqqqqqqqqqqqqqqg==")]
    public void Validate_TooLongString_ThrowsException(string encoded, IvValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(encoded));
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData("qqqqqqqqqqqqqgg")]
    [InlineAutoData("qqqqqqqqqqo=")]
    public void Validate_TooShortString_ThrowsException(string encoded, IvValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(encoded));
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData("-")]
    [InlineAutoData("\\")]
    [InlineAutoData("å")]
    [InlineAutoData(";")]
    [InlineAutoData("+/")]
    [InlineAutoData("+/=")]
    [InlineAutoData("+/===")]
    [InlineAutoData("+/====")]
    [InlineAutoData("qqqqqqqqqqqqq===")]
    public void Validate_GivenIllegalBase64_ThrowsException(string value, IvValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(value));
        validating.Should().Throw<ValidationException>();
    }

    [Theory, InlineAutoData("lAcXa6oM5PZY9qOO")]
    public void Validate_GivenValidInitializationVector_ReturnsValue(string encoded, IvValidator sut)
        => sut.Validate(new UntrustedValue<string>(encoded)).Should().Be(encoded);
}
