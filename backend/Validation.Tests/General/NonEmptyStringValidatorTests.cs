// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Validation.General;
using Xunit;

namespace Validation.Tests.General;

public class NonEmptyStringValidatorTests
{
    [Theory]
    [InlineAutoData("")]
    [InlineAutoData(" ")]
    [InlineAutoData(null)]
    internal void Validate_GivenNullOrEmptyString_ThrowsException(string value, NonEmptyStringValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        Action validating = () => sut.Validate(untrusted);
        validating.Should().Throw<ValidationException>();
    }

    [Theory, AutoData]
    internal void Validate_GivenStringWithContents_ReturnsValue(string value, NonEmptyStringValidator sut)
    {
        var untrusted = new UntrustedValue<string>(value);
        sut.Validate(untrusted).Should().Be(value);
    }
}
