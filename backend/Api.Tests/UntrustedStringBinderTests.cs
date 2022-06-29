using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Validation;
using Xunit;

namespace Api.Tests;

public class UntrustedStringBinderTests
{
    private readonly Fixture fixture = new();
    private UntrustedValue<string>? model;

    [Theory, AutoData]
    internal void BindModelAsync_GivenNullBindingContext_ThrowsException(UntrustedStringBinder sut)
    {
        Action binding = () => sut.BindModelAsync(null);
        binding.Should().Throw<ArgumentNullException>();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenNoValue_ReturnsCompletedTask(UntrustedStringBinder sut)
    {
        var (context, _) = CreateBinderContext(ValueProviderResult.None);
        sut.BindModelAsync(context).IsCompleted.Should().BeTrue();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenNoValue_DoesNotChangeState(UntrustedStringBinder sut)
    {
        var (context, state) = CreateBinderContext(ValueProviderResult.None);
        sut.BindModelAsync(context);
        state.Should().BeEmpty();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenValue_ReturnsCompletedTask(ValueProviderResult result, UntrustedStringBinder sut)
    {
        var (context, _) = CreateBinderContext(result);
        sut.BindModelAsync(context).IsCompleted.Should().BeTrue();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenValue_ChangesState(ValueProviderResult result, UntrustedStringBinder sut)
    {
        var (context, state) = CreateBinderContext(result);
        sut.BindModelAsync(context);
        state.Should().NotBeEmpty();
    }

    [Theory, AutoData]
    internal void BindModelAsync_GivenValue_ReturnsModel(string actualValue, UntrustedStringBinder sut)
    {
        var result = new ValueProviderResult(actualValue);
        var (context, _) = CreateBinderContext(result);

        sut.BindModelAsync(context);

        model.Should().NotBeNull()
            .And.Subject.Equals(actualValue).Should().BeTrue();
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
            .Callback((ModelBindingResult x) => { model = x.Model as UntrustedValue<string>; });

        return (context.Object, state);
    }

    private static IValueProvider MockProviderWithModelValue(string modelName, ValueProviderResult value)
    {
        var valueProvider = new Mock<IValueProvider>();
        valueProvider.Setup(x => x.GetValue(modelName)).Returns(value);
        return valueProvider.Object;
    }
}
