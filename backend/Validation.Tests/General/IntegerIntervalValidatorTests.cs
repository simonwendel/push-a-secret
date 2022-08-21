// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Validation.General;
using Xunit;

namespace Validation.Tests.General;

public class IntegerIntervalValidatorTests
{
    [Theory]
    [InlineData(10, 9)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    internal void Ctor_GivenInvalidBounds_ThrowsArgumentException(int lowerBounds, int upperBounds)
    {
        Action constructing = () => new IntegerIntervalValidator(lowerBounds, upperBounds);
        constructing.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(-1, 9)]
    [InlineData(0, -1)]
    internal void Ctor_GivenNegativeBounds_ThrowsArgumentOutOfRangeException(int lowerBounds, int upperBounds)
    {
        Action constructing = () => new IntegerIntervalValidator(lowerBounds, upperBounds);
        constructing.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineAutoData(0, 1)]
    [InlineAutoData(3, 4)]
    [InlineAutoData(5, 6)]
    internal void Validate_GivenValueLessThanLowerBounds_ThrowsException(int value, int lowerBounds)
    {
        var untrusted = new UntrustedValue<int>(value);
        var sut = new IntegerIntervalValidator(lowerBounds);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData(2, 0, 1)]
    [InlineAutoData(5, 2, 4)]
    [InlineAutoData(21, 6, 20)]
    internal void Validate_GivenValueGreaterThenUpperBounds_ThrowsException(int value, int lowerBounds, int upperBounds)
    {
        var untrusted = new UntrustedValue<int>(value);
        var sut = new IntegerIntervalValidator(lowerBounds, upperBounds);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }

    [Fact]
    internal void Validate_GivenValueWithinBounds_ReturnsValue()
    {
        var expected = 5;
        var untrusted = new UntrustedValue<int>(expected);
        var sut = new IntegerIntervalValidator(4, 6);
        sut.Validate(untrusted).Should().Be(expected);
    }
}
