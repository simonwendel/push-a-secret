// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using Domain;
using FluentAssertions;
using Validation.Specific;
using Xunit;

namespace Validation.Tests.Specific;

public class SecretValidatorTests
{
    private readonly Secret validSecret = new("A128GCM", "xMY6HokU51VT8g02", "zb8HP7LFqYr+1fWZA5ZFAfIHz3Y=", 4);

    private readonly SecretValidator sut = new(
        new AlgorithmValidator(),
        new IvValidator(),
        new CiphertextValidator(),
        new TtlDaysValidator());

    [Fact]
    internal void Validate_GivenValidSecret_ReturnsValue()
        => sut.Validate(new UntrustedValue<Secret>(validSecret)).Should().Be(validSecret);

    [Fact]
    internal void Validate_GivenInvalidAlgorithm_ThrowsException()
        => EnsureInvalid(validSecret with {Algorithm = "A128CTR"});

    [Fact]
    internal void Validate_GivenInvalidIv_ThrowsException()
        => EnsureInvalid(validSecret with {IV = "xMY6Hok1VT8g0"});

    [Fact]
    internal void Validate_GivenInvalidCiphertext_ThrowsException()
        => EnsureInvalid(validSecret with {Ciphertext = "zb8HP7LFqYr+1fWZA5ZFAfIHz3Y"});

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    internal void Validate_GivenInvalidTtl_ThrowsException(int ttl)
        => EnsureInvalid(validSecret with {Ttl = ttl});

    private void EnsureInvalid(Secret secret)
    {
        Action validating = () => sut.Validate(new UntrustedValue<Secret>(secret));
        validating.Should().Throw<ValidationException>();
    }
}
