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
        var encoded = GetLongestValidEncodedCipher();
        sut.Validate(new UntrustedValue<string>(encoded)).Should().Be(encoded);
    }

    private static string GetLongestValidEncodedCipher()
    {
        const int maxNumberOfEncryptedBytes = 72 * 2 + 16; // 72 characters * 2 bytes per cp + 16 bytes auth tag
        var correspondingCipher = Enumerable.Repeat((byte) 0xFF, maxNumberOfEncryptedBytes).ToArray();
        return Convert.ToBase64String(correspondingCipher);
    }
}
