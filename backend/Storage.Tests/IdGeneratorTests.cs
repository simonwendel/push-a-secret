using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Xunit;

namespace Storage.Tests;

public class IdGeneratorTests
{
    private readonly Mock<ITimestampGenerator> timestampGenerator = new();
    private readonly Mock<IBase36Converter> baseConverter = new();
    private readonly IdGenerator sut;

    public IdGeneratorTests()
        => sut = new IdGenerator(timestampGenerator.Object, baseConverter.Object);

    [Theory, AutoData]
    public void Generate_UsesTimestamp_ReturnsBase36Conversion(long timestamp, string converted)
    {
        timestampGenerator.Setup(x => x.Generate()).Returns(timestamp);
        baseConverter.Setup(x => x.ToBase36(timestamp)).Returns(converted);

        sut.Generate().Should().Be(converted);

        timestampGenerator.VerifyAll();
        baseConverter.VerifyAll();
    }
}
