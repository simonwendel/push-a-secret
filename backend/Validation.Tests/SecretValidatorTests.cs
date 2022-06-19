using System;
using Domain;
using FluentAssertions;
using Xunit;

namespace Validation.Tests;

public class SecretValidatorTests
{
    private readonly Secret validSecret = new("A128GCM", "xMY6HokU51VT8g02", "zb8HP7LFqYr+1fWZA5ZFAfIHz3Y=");
    private readonly SecretValidator sut = new();

    [Fact]
    public void Validate_GivenValidSecret_ReturnsValue()
        => sut.Validate(new UntrustedValue<Secret>(validSecret)).Should().Be(validSecret);

    [Fact]
    public void Validate_GivenInvalidAlgorithm_ThrowsException()
        => EnsureInvalid(validSecret with {Algorithm = "A128CTR"});

    [Fact]
    public void Validate_GivenInvalidIv_ThrowsException()
        => EnsureInvalid(validSecret with {IV = "xMY6Hok1VT8g0"});

    [Fact]
    public void Validate_GivenInvalidCiphertext_ThrowsException()
        => EnsureInvalid(validSecret with {Ciphertext = "zb8HP7LFqYr+1fWZA5ZFAfIHz3Y"});

    private void EnsureInvalid(Secret secret)
    {
        Action validating = () => sut.Validate(new UntrustedValue<Secret>(secret));
        validating.Should().Throw<ValidationException>();
    }
}
