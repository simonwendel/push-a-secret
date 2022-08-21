// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Api.Tests;

public class ServerErrorMiddlewareTests
{
    private static readonly HttpContext context = Mock.Of<HttpContext>();
    private static readonly Mock<RequestDelegate> next = new();

    [Fact]
    internal async Task InvokeAsync_GivenRequestDelegate_InvokesDelegate()
    {
        var sut = new ServerErrorMiddleware(next.Object);
        await sut.InvokeAsync(context);
        next.Verify(x => x(context), Times.Once);
    }
}
