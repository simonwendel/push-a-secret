using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Validation;
using Xunit;

namespace Api.Tests;

public class UntrustedValueBinderProviderTests
{
    [Theory, AutoData]
    public void GetBinder_GivenNullContext_ThrowsException(UntrustedValueBinderProvider sut)
    {
        Action getting = () => sut.GetBinder(null);
        getting.Should().Throw<ArgumentNullException>();
    }

    [Theory, AutoData]
    public void GetBinder_GivenWrongType_ReturnsNull(Type type, UntrustedValueBinderProvider sut)
    {
        var context = GetProviderContextFor(type);
        sut.GetBinder(context).Should().BeNull();
    }

    [Theory, AutoData]
    public void GetBinder_GivenUntrustedValueOfString_ReturnsBinder(UntrustedValueBinderProvider sut)
    {
        var context = GetProviderContextFor(typeof(UntrustedValue<string>));
        sut.GetBinder(context).Should().BeOfType<UntrustedStringBinder>();
    }

    private static ModelBinderProviderContext GetProviderContextFor(Type type)
    {
        var factory = new EmptyModelMetadataProvider();
        var metadata = factory.GetMetadataForType(type);

        var context = new Mock<ModelBinderProviderContext>();
        context.Setup(x => x.Metadata).Returns(metadata);

        return context.Object;
    }
}
