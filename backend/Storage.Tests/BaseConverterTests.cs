using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Storage.Tests;

public class BaseConverterTests
{
    [Theory]
    [InlineAutoData(0, "0")]
    [InlineAutoData(1, "1")]
    [InlineAutoData(28, "s")]
    [InlineAutoData(35, "z")]
    [InlineAutoData(36, "10")]
    [InlineAutoData(37, "11")]
    [InlineAutoData(1337, "115")]
    [InlineAutoData(1654023319428, "l3uis1ac")]
    [InlineAutoData(long.MaxValue, "1y2p0ij32e8e7")]
    public void ToBase36_GivenPositiveNumber_ReturnsStringInBase36(long number, string expected, Base36Converter sut)
    {
        sut.ToBase36(number).Should().Be(expected);
    }

    [Theory]
    [InlineAutoData(-1)]
    [InlineAutoData(long.MinValue)]
    public void ToBase36_GivenNegativeNumber_ThrowsException(long number, Base36Converter sut)
    {
        Action converting = () => sut.ToBase36(number);
        converting.Should().Throw<ArgumentOutOfRangeException>();
    }
}