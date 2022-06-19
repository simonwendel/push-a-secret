using System;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Validation.Tests;

public class CiphertextValidatorTests
{
    [Fact]
    public void MaxCipherLength_AsPerConfiguration_ShouldBe216()
        => CiphertextValidator.MaxCipherLength.Should().Be(216);

    [Fact]
    public void MinCipherLength_AsPerConfiguration_ShouldBe24()
        => CiphertextValidator.MinCipherLength.Should().Be(24);

    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(" ")]
    [InlineAutoData("\n")]
    [InlineAutoData("\t")]
    public void Validate_GivenEmptyString_ThrowsException(string value, CiphertextValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(value));
        validating.Should().Throw<ValidationException>();
    }

    [Theory, AutoData]
    public void Validate_TooLongString_ThrowsException(CiphertextValidator sut)
    {
        var tooLong = $"{new string('+', CiphertextValidator.MaxCipherLength + 1)}===";
        Action validating = () => sut.Validate(new UntrustedValue<string>(tooLong));
        validating.Should().Throw<ValidationException>();
    }

    [Theory, AutoData]
    public void Validate_TooShortString_ThrowsException(CiphertextValidator sut)
    {
        var tooLong = $"{new string('+', CiphertextValidator.MinCipherLength - 1)}";
        Action validating = () => sut.Validate(new UntrustedValue<string>(tooLong));
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
    public void Validate_GivenIllegalBase64_ThrowsException(string value, CiphertextValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(value));
        validating.Should().Throw<ValidationException>();
    }

    [Theory, AutoData]
    public void Validate_GivenLongestValidEncoding_ReturnsValue(CiphertextValidator sut)
    {
        var encoded = GetValidEncodedCipherOfLength(72);
        sut.Validate(new UntrustedValue<string>(encoded)).Should().Be(encoded);
    }

    [Theory, AutoData]
    public void Validate_GivenShortestValidEncoding_ReturnsValue(CiphertextValidator sut)
    {
        var encoded = GetValidEncodedCipherOfLength(1);
        sut.Validate(new UntrustedValue<string>(encoded)).Should().Be(encoded);
    }

    private static string GetValidEncodedCipherOfLength(int length)
    {
        var maxNumberOfEncryptedBytes = length * 2 + 16;
        var correspondingCipher = Enumerable.Repeat((byte) 0xFF, maxNumberOfEncryptedBytes).ToArray();
        return Convert.ToBase64String(correspondingCipher);
    }
}
