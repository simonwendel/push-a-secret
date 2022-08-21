// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using FluentAssertions;
using Validation.General;
using Xunit;

namespace Validation.Tests.General;

public class NonEmptyStringLengthValidatorTests
{
    [Theory]
    [InlineData("12")]
    [InlineData("123456")]
    [InlineData("    ")]
    [InlineData("\t\t\t\t")]
    internal void Validate_GivenNonValidString_ThrowsException(string value)
    {
        var untrusted = new UntrustedValue<string>(value);
        var sut = new NonEmptyStringLengthValidator(3, 5);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    [InlineData("   1")]
    [InlineData("\t\ta\t")]
    internal void Validate_GivenValidString_ReturnsValue(string value)
    {
        var untrusted = new UntrustedValue<string>(value);
        var sut = new NonEmptyStringLengthValidator(3, 5);
        sut.Validate(untrusted).Should().Be(value);
    }
}
