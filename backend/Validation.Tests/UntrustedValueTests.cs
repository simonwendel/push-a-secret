using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Validation.Tests;

public class UntrustedValueTests
{
    private readonly SuperSecretTestClass value = new();
    private readonly UntrustedValue<SuperSecretTestClass> untrusted;

    public UntrustedValueTests()
        => untrusted = new(value);

    [Theory, AutoData]
    public void Equals_Always_ReturnsEqualsForWrappedValue(object other)
    {
        value.Equals(other).Should().Be(value.EqualsResult);
        value.EqualsCalled.Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_Always_ReturnsHashCodeForWrappedValue()
    {
        untrusted.GetHashCode().Should().Be(value.GetHashCodeResult);
        value.GetHashCodeCalled.Should().BeTrue();
    }

    [Fact]
    public void ToString_Always_ThrowsExceptionInsteadOfReturningValue()
    {
        var rendering = () =>
        {
            var _ = untrusted.ToString();
        };

        rendering.Should().Throw<InvalidOperationException>();
        value.ToStringCalled.Should().BeFalse();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    private class SuperSecretTestClass
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
