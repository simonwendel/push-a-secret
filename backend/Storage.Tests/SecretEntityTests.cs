// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using AutoFixture.Xunit2;
using Xunit;
using FluentAssertions;

namespace Storage.Tests;

public class SecretEntityTests
{
    /// <remarks>
    /// We never want code to not set a property by accident, thus this must work
    /// in order to verify that no exceptions are thrown in the happy path.
    /// </remarks>
    [Theory, AutoData]
    internal void PropertiesWithNullableBackingFields_WhenSet_CanBeRead(
        string id,
        string algorithm,
        string iv,
        string ciphertext,
        int ttl)
    {
        var sut = new SecretEntity
        {
            Id = id,
            Algorithm = algorithm,
            IV = iv,
            Ciphertext = ciphertext,
            Ttl = ttl
        };

        sut.Id.Should().Be(id);
        sut.Algorithm.Should().Be(algorithm);
        sut.IV.Should().Be(iv);
        sut.Ciphertext.Should().Be(ciphertext);
        sut.Ttl.Should().Be(ttl);
    }

    /// <remorks>
    /// Sadly we need the init properties for the MongoDB driver, so in order to always
    /// make sure we have values for things, we make this explicit by throwing an exception.
    /// </remorks>
    [Fact]
    internal void PropertiesWithNullableBackingFields_WhenNotSet_CantBeRead()
    {
        var sut = new SecretEntity();
        EnsureWeCantDo(() => sut.Id);
        EnsureWeCantDo(() => sut.Algorithm);
        EnsureWeCantDo(() => sut.IV);
        EnsureWeCantDo(() => sut.Ciphertext);
        EnsureWeCantDo(() => sut.Ttl);
    }

    private static void EnsureWeCantDo<T>(Func<T> action)
        => action.Should().Throw<InvalidOperationException>();
}
