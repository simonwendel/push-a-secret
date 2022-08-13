using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Validation.Tests;

public class UntrustedValueTests
{
    private readonly SuperSecretTestClass value = new();
    private readonly UntrustedValue<SuperSecretTestClass> sut;

    public UntrustedValueTests()
        => sut = new UntrustedValue<SuperSecretTestClass>(value);

    [Theory, AutoData]
    internal void Equals_GivenWrappedValue_ReturnsEqualsForWrappedValue(SuperSecretTestClass other)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        sut.Equals(other).Should().Be(value.EqualsResult);
        value.EqualsCalled.Should().BeTrue();
    }

    [Theory]
    [InlineAutoData("1", "2", false)]
    [InlineAutoData("2", "1", false)]
    [InlineAutoData("2", "2", true)]
    internal void Equals_GivenUntrustedValue_ReturnsEqualsForWrappedValues(string first, string second, bool expected)
    {
        var @this = new UntrustedValue<string>(first);
        var that = new UntrustedValue<string>(second);
        @this.Equals(that).Should().Be(expected);
    }

    [Theory, AutoData]
    internal void Equals_GivenNull_ReturnsFalse(UntrustedValue<object> sut)
    {
        sut.Equals(null).Should().BeFalse();
    }

    [Fact]
    internal void GetHashCode_Always_ReturnsHashCodeForWrappedValue()
    {
        sut.GetHashCode().Should().Be(value.GetHashCodeResult);
        value.GetHashCodeCalled.Should().BeTrue();
    }

    [Fact(Skip = "We don't use this in debug builds anymore.")]
    internal void ToString_Always_ThrowsExceptionInsteadOfReturningValue()
    {
        var rendering = () =>
        {
            var _ = sut.ToString();
        };

        rendering.Should().Throw<InvalidOperationException>();
        value.ToStringCalled.Should().BeFalse();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    internal class SuperSecretTestClass
    {
        internal bool EqualsCalled { get; private set; }
        internal bool EqualsResult { get; }

        internal bool GetHashCodeCalled { get; private set; }
        internal int GetHashCodeResult { get; }

        internal bool ToStringCalled { get; private set; }
        private string ToStringValue { get; }

        public SuperSecretTestClass()
        {
            EqualsCalled = false;
            EqualsResult = true;

            GetHashCodeCalled = false;
            GetHashCodeResult = 1337;

            ToStringCalled = false;
            ToStringValue = "1337";
        }

        public override bool Equals(object? obj)
        {
            EqualsCalled = true;
            return EqualsResult;
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            GetHashCodeCalled = true;
            return GetHashCodeResult;
        }

        public override string ToString()
        {
            ToStringCalled = true;
            return ToStringValue;
        }
    }
}
