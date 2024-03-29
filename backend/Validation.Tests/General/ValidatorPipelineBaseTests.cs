﻿// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using FluentAssertions;
using Validation.General;
using Xunit;

namespace Validation.Tests.General;

public class ValidatorPipelineBaseTests
{
    [Fact]
    internal void Ctor_GivenNoValidators_ThrowsException()
    {
        var constructing = () => new TestPipeline();
        constructing.Should().Throw<ArgumentException>();
    }

    private class TestPipeline : ValidatorPipelineBase<object>
    {
        public TestPipeline()
            : base(Array.Empty<ValidatorBase<object>>())
        {
        }
    }
}
