// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Xunit;

namespace Domain.Tests;

public class IdGeneratorTests
{
    private readonly Mock<ITimestampGenerator> timestampGenerator = new();
    private readonly Mock<IBaseConverter> baseConverter = new();
    private readonly IdGenerator sut;

    public IdGeneratorTests()
        => sut = new IdGenerator(timestampGenerator.Object, baseConverter.Object);

    [Theory, AutoData]
    internal void Generate_UsesTimestamp_ReturnsBase36Conversion(long timestamp, string converted)
    {
        timestampGenerator.Setup(x => x.Generate()).Returns(timestamp);
        baseConverter.Setup(x => x.ToBase36(timestamp)).Returns(converted);

        sut.Generate().Should().Be(new Identifier(converted));

        timestampGenerator.VerifyAll();
        baseConverter.VerifyAll();
    }
}
