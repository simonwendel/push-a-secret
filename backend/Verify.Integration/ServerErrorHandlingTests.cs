// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Verify.Integration;

[Collection("Verify.Integration")]
public class ServerErrorHandlingTests
{
    private readonly HttpClient crashingClient;

    public ServerErrorHandlingTests()
    {
        var crashingApp = new ApiApplication(crashOnRequest: true);
        crashingClient = crashingApp.CreateClient();
    }

    [Fact]
    internal async Task Request_WhenExceptionIsThrown_ReturnsInternalServerError()
    {
        var response = await crashingClient.GetAsync("secret/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    internal async Task Request_WhenExceptionIsThrown_ReturnsProblemDetailsAsJson()
    {
        var response = await crashingClient.GetAsync("secret/1");
        var details = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        details.Should().NotBeNull();
        details!.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.6.1");
        details.Title.Should().Be("An error occurred while processing your request.");
        details.Status.Should().Be((int) HttpStatusCode.InternalServerError);

        details.Extensions.Should().ContainKey("traceId").WhoseValue.As<string>().Should().NotBeEmpty();
    }
}
