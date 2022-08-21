// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Validation.Specific;
using Xunit;

namespace Validation.Tests.Specific;

public class IvValidatorTests
{
    [Fact]
    internal void IvLength_AsPerConfiguration_ShouldBe16()
        => IvValidator.IvLength.Should().Be(16);

    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(" ")]
    [InlineAutoData("\n")]
    [InlineAutoData("\t")]
    internal void Validate_GivenEmptyString_ThrowsException(string value, IvValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(value));
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData("qqqqqqqqqqqqqqqqq")]
    [InlineAutoData("qqqqqqqqqqqqqqqqqg==")]
    internal void Validate_TooLongString_ThrowsException(string encoded, IvValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(encoded));
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData("qqqqqqqqqqqqqgg")]
    [InlineAutoData("qqqqqqqqqqo=")]
    internal void Validate_TooShortString_ThrowsException(string encoded, IvValidator sut)
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
    internal void Validate_GivenIllegalBase64_ThrowsException(string value, IvValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(value));
        validating.Should().Throw<ValidationException>();
    }

    [Theory, InlineAutoData("lAcXa6oM5PZY9qOO")]
    internal void Validate_GivenValidInitializationVector_ReturnsValue(string encoded, IvValidator sut)
        => sut.Validate(new UntrustedValue<string>(encoded)).Should().Be(encoded);
}
