using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Domain.Tests;

public class Base64StatisticsTests
{
    [Theory]
    [InlineAutoData(-1)]
    [InlineAutoData(int.MaxValue / 2 + 1)]
    public void GetInflation_GivenInvalidNumberOfBytes_ThrowsException(int numberOfBytes)
    {
        Action getting = () => Base64Statistics.GetInflationFor(numberOfBytes);
        getting.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineAutoData(0, 0)]
    [InlineAutoData(1, 4)]
    [InlineAutoData(2, 4)]
    [InlineAutoData(3, 4)]
    [InlineAutoData(4, 8)]
    [InlineAutoData(5, 8)]
    [InlineAutoData(6, 8)]
    [InlineAutoData(19, 28)]
    [InlineAutoData(160, 216)]
    [InlineAutoData(int.MaxValue / 2, 1431655764)]
    public void GetInflation_GivenNumberOfBytes_CalculatesInflation(int numberOfBytes, int expected)
        => Base64Statistics.GetInflationFor(numberOfBytes).Should().Be(expected);
}
