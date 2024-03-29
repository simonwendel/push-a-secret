﻿// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Validation;
using Xunit;

namespace Api.Tests;

public class UntrustedValueBinderProviderTests
{
    [Theory, AutoData]
    internal void GetBinder_GivenNullContext_ThrowsException(UntrustedValueBinderProvider sut)
    {
        Action getting = () => sut.GetBinder(null);
        getting.Should().Throw<ArgumentNullException>();
    }

    [Theory, AutoData]
    internal void GetBinder_GivenWrongType_ReturnsNull(Type type, UntrustedValueBinderProvider sut)
    {
        var context = GetProviderContextFor(type);
        sut.GetBinder(context).Should().BeNull();
    }

    [Theory, AutoData]
    internal void GetBinder_GivenUntrustedValueOfIdentifier_ReturnsBinder(UntrustedValueBinderProvider sut)
    {
        var context = GetProviderContextFor(typeof(UntrustedValue<Identifier>));
        sut.GetBinder(context).Should().BeOfType<UntrustedIdentifierBinder>();
    }

    [Theory, AutoData]
    internal void GetBinder_GivenUntrustedValueOfSecret_ReturnsBinder(UntrustedValueBinderProvider sut)
    {
        var context = GetProviderContextFor(typeof(UntrustedValue<Secret>));
        sut.GetBinder(context).Should().BeOfType<UntrustedSecretBinder>();
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
