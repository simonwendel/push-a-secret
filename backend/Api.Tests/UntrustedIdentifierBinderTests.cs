using System;
using AutoFixture;
using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Validation;
using Xunit;

namespace Api.Tests;

public class UntrustedIdentifierBinderTests
{
    private readonly Fixture fixture = new();
    private UntrustedValue<Identifier>? model;

    [Theory, AutoData]
    internal void BindModelAsync_GivenNullBindingContext_ThrowsException(UntrustedIdentifierBinder sut)
    {
        Action binding = () => sut.BindModelAsync(null);
        binding.Should().Throw<ArgumentNullException>();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenNoValue_ReturnsCompletedTask(UntrustedIdentifierBinder sut)
    {
        var (context, _) = CreateBinderContext(ValueProviderResult.None);
        sut.BindModelAsync(context).IsCompleted.Should().BeTrue();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenNoValue_DoesNotChangeState(UntrustedIdentifierBinder sut)
    {
        var (context, state) = CreateBinderContext(ValueProviderResult.None);
        sut.BindModelAsync(context);
        state.Should().BeEmpty();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenValue_ReturnsCompletedTask(ValueProviderResult result, UntrustedIdentifierBinder sut)
    {
        var (context, _) = CreateBinderContext(result);
        sut.BindModelAsync(context).IsCompleted.Should().BeTrue();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenValue_ChangesState(ValueProviderResult result, UntrustedIdentifierBinder sut)
    {
        var (context, state) = CreateBinderContext(result);
        sut.BindModelAsync(context);
        state.Should().NotBeEmpty();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenValue_ReturnsModel(string actualValue, UntrustedIdentifierBinder sut)
    {
        var result = new ValueProviderResult(actualValue);
        var (context, _) = CreateBinderContext(result);
        var expected = new Identifier(actualValue);

        sut.BindModelAsync(context);

        model.Should().NotBeNull()
            .And.Subject.Equals(expected).Should().BeTrue();
    }

    private (ModelBindingContext, ModelStateDictionary) CreateBinderContext(
        ValueProviderResult value)
    {
        var modelName = fixture.Create<string>();
        var provider = MockProviderWithModelValue(modelName, value);
        return MockBindingContextWith(modelName, provider);
    }

    private (ModelBindingContext, ModelStateDictionary) MockBindingContextWith(
        string modelName,
        IValueProvider valueProvider)
    {
        var state = new ModelStateDictionary();

        var context = new Mock<ModelBindingContext>();
        context.Setup(x => x.ModelName).Returns(modelName);
        context.Setup(x => x.ValueProvider).Returns(valueProvider);
        context.Setup(x => x.ModelState).Returns(state);
        context.SetupSet(x => x.Result = It.IsAny<ModelBindingResult>())
            .Callback((ModelBindingResult x) => { model = x.Model as UntrustedValue<Identifier>; });

        return (context.Object, state);
    }

    private static IValueProvider MockProviderWithModelValue(string modelName, ValueProviderResult value)
    {
        var valueProvider = new Mock<IValueProvider>();
        valueProvider.Setup(x => x.GetValue(modelName)).Returns(value);
        return valueProvider.Object;
    }
}
