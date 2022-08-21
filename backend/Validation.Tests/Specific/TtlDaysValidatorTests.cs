// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Validation.Specific;
using Xunit;

namespace Validation.Tests.Specific;

public class TtlDaysValidatorTests
{
    [Fact]
    internal void MinTtl_AsPerConfiguration_ShouldBe1()
        => TtlDaysValidator.MinTtl.Should().Be(1);

    [Fact]
    internal void MaxTtl_AsPerConfiguration_ShouldBe7()
        => TtlDaysValidator.MaxTtl.Should().Be(7);

    [Theory, AutoData]
    internal void Validate_GivenIntegerWithinValidInterval_ReturnsValue(TtlDaysValidator sut)
    {
        var validInterval = Enumerable.Range(TtlDaysValidator.MinTtl, TtlDaysValidator.MaxTtl);
        foreach (var expected in validInterval)
        {
            var untrusted = new UntrustedValue<int>(expected);
            sut.Validate(untrusted).Should().Be(expected);
        }
    }

    [Theory]
    [InlineAutoData(TtlDaysValidator.MinTtl - 1)]
    [InlineAutoData(TtlDaysValidator.MaxTtl + 1)]
    internal void Validate_GivenIntegerOutsideValidInterval_ThrowsException(int value, TtlDaysValidator sut)
    {
        var untrusted = new UntrustedValue<int>(value);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }
}
