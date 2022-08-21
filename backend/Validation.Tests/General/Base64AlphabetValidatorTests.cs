// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Validation.General;
using Xunit;

namespace Validation.Tests.General;

public class Base64AlphabetValidatorTests
{
    [Theory]
    [InlineAutoData("-")]
    [InlineAutoData("\\")]
    [InlineAutoData("å")]
    [InlineAutoData(";")]
    internal void Validate_GivenIllegalCharacter_ThrowsException(string untrusted, Base64AlphabetValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(untrusted));
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData("+/")]
    [InlineAutoData("+/=")]
    [InlineAutoData("+/===")]
    [InlineAutoData("+/====")]
    internal void Validate_GivenInvalidPadding_ThrowsException(string value, Base64AlphabetValidator sut)
    {
        Action validating = () => sut.Validate(new UntrustedValue<string>(value));
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData("ABCDEFGHIJKLMNOPQRSTUVWXYZ==")]
    [InlineAutoData("abcdefghijklmnopqrstuvwxyz==")]
    [InlineAutoData("0123456789==")]
    [InlineAutoData("+/==")]
    [InlineAutoData(" ")]
    [InlineAutoData("")]
    internal void Validate_GivenValidString_ReturnsValue(string value, Base64AlphabetValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        sut.Validate(untrusted).Should().Be(value);
    }
}
