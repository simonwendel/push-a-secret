using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Conversion.Tests;

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
    internal void ToBase36_GivenPositiveNumber_ReturnsStringInBase36(long number, string expected, BaseConverter sut)
    {
        sut.ToBase36(number).Should().Be(expected);
    }

    [Theory]
    [InlineAutoData(-1)]
    [InlineAutoData(long.MinValue)]
    internal void ToBase36_GivenNegativeNumber_ThrowsException(long value, BaseConverter sut)
    {
        Action converting = () => sut.ToBase36(value);
        converting.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineAutoData("0", 0)]
    [InlineAutoData("1", 1)]
    [InlineAutoData("s", 28)]
    [InlineAutoData("S", 28)]
    [InlineAutoData("z", 35)]
    [InlineAutoData("Z", 35)]
    [InlineAutoData("10", 36)]
    [InlineAutoData("11", 37)]
    [InlineAutoData("115", 1337)]
    [InlineAutoData("l3uis1ac", 1654023319428)]
    [InlineAutoData("L3UIS1AC", 1654023319428)]
    [InlineAutoData("1y2p0ij32e8e7", long.MaxValue)]
    [InlineAutoData("1Y2P0IJ32E8E7", long.MaxValue)]
    internal void FromBase36_GivenStringInBase36_ReturnsLongValue(string value, long expected, BaseConverter sut)
    {
        sut.FromBase36(value).Should().Be(expected);
    }

    [Theory]
    [InlineAutoData("-1")]
    [InlineAutoData("")]
    [InlineAutoData(" ")]
    [InlineAutoData(";")]
    [InlineAutoData("abc;")]
    [InlineAutoData("aBC ")]
    [InlineAutoData(";")]
    [InlineAutoData("-9223372036854775808")]
    internal void FromBase36_GivenInvalidString_ThrowsException(string base36, BaseConverter sut)
    {
        Action converting = () => sut.FromBase36(base36);
        converting.Should().Throw<ArgumentException>();
    }
}
