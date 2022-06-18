using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Validation.Tests;

public class AlgorithmValidatorTests
{
    [Fact]
    public void Default_AsPerConfiguration_ShouldBeAesGcm128()
        => AlgorithmValidator.DefaultAlgorithm.Should().Be("A128GCM");

    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(" ")]
    [InlineAutoData("\n")]
    [InlineAutoData("\t")]
    [InlineAutoData("A256GCM")]
    public void Validate_GivenOtherThanDefaultAlgorithm_ThrowsException(string algorithm, AlgorithmValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(algorithm));
        validating.Should().Throw<ValidationException>();
    }

    [Theory, InlineAutoData("A128GCM")]
    public void Validate_GivenDefaultAlgorithm_ReturnsValue(string algorithm, AlgorithmValidator sut)
        => sut.Validate(new UntrustedValue<string>(algorithm)).Should().Be(algorithm);
}
