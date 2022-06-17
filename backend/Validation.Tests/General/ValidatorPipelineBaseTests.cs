using System;
using FluentAssertions;
using Validation.General;
using Xunit;

namespace Validation.Tests.General;

public class ValidatorPipelineBaseTests
{
    [Fact]
    public void Ctor_GivenNoValidators_ThrowsException()
    {
        var constructing = () =>
        {
            var pipeline = new TestPipeline();
        };

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
