// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Validation.Specific;
using Xunit;

namespace Validation.Tests.Specific;

public class AlgorithmValidatorTests
{
    [Fact]
    internal void Default_AsPerConfiguration_ShouldBeAesGcm128()
        => AlgorithmValidator.DefaultAlgorithm.Should().Be("A128GCM");

    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(" ")]
    [InlineAutoData("\n")]
    [InlineAutoData("\t")]
    [InlineAutoData("A256GCM")]
    internal void Validate_GivenOtherThanDefaultAlgorithm_ThrowsException(string algorithm, AlgorithmValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(algorithm));
        validating.Should().Throw<ValidationException>();
    }

    [Theory, InlineAutoData("A128GCM")]
    internal void Validate_GivenDefaultAlgorithm_ReturnsValue(string algorithm, AlgorithmValidator sut)
        => sut.Validate(new UntrustedValue<string>(algorithm)).Should().Be(algorithm);
}
