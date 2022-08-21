// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Validation.General;
using Xunit;

namespace Validation.Tests.General;

public class ObjectPropertyValidatorTests
{
    [Fact]
    internal void Validate_GivenInvalidProperty_ThrowsException()
    {
        var (sut, obj) = CreateSystemFor(string.Empty);
        Action validating = () => sut.Validate(new UntrustedValue<TestValue>(obj));
        validating.Should().Throw<ValidationException>();
    }

    [Theory, AutoData]
    internal void Validate_GivenValidProperty_ReturnsValue(string value)
    {
        var (sut, obj) = CreateSystemFor(value);
        sut.Validate(new UntrustedValue<TestValue>(obj)).TestProperty.Should().Be(value);
    }

    private class TestValue
    {
        public TestValue(string testProperty)
        {
            TestProperty = testProperty;
        }

        public string TestProperty { get; }
    }

    private (ObjectPropertyValidator<TestValue, string>, TestValue) CreateSystemFor(string value)
    {
        var validator = new NonEmptyStringValidator();
        var obj = new TestValue(value);
        var sut = new ObjectPropertyValidator<TestValue, string>(x => x.TestProperty, validator);
        return (sut, obj);
    }
}
