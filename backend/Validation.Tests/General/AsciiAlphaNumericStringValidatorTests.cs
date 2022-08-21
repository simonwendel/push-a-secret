// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Validation.General;
using Xunit;

namespace Validation.Tests.General;

public class AsciiAlphaNumericStringValidatorTests
{
    [Theory]
    [InlineAutoData(" ")]
    [InlineAutoData("å")]
    [InlineAutoData(";")]
    [InlineAutoData("123_")]
    [InlineAutoData("😊")]
    internal void Validate_GivenStringWithNonAlphanumericCharacters_ThrowsException(string value, AsciiAlphaNumericStringValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineAutoData("1")]
    [InlineAutoData("a")]
    [InlineAutoData("0123456789abcdefghijklmnopqrstuvxyz")]
    internal void Validate_GivenStringWithContents_ReturnsValue(string value, AsciiAlphaNumericStringValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        sut.Validate(untrusted).Should().Be(value);
    }
}
