using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Domain;
using FluentAssertions;
using Xunit;

namespace Verify.Integration;

[Collection("Verify.Integration")]
public class EndToEndIntegrationTests
{
    private readonly Secret secret;
    private readonly HttpClient client;

    public EndToEndIntegrationTests()
    {
        var app = new ApiApplication();
        client = app.CreateClient();
        secret = new("A128GCM", "xMY6HokU51VT8g02", "zb8HP7LFqYr+1fWZA5ZFAfIHz3Y=");
    }

    [Fact]
    internal async Task RoundTrip_WhenRunAgainstApi_WorksAsIntended()
        => await client
            .InsertNew(secret)
            .VerifyItExists()
            .VerifyItCanBeRead()
            .DeleteIt()
            .VerifyItIsGone();
}

internal static class FluentEndToEndIntegrationTestMethods
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<(HttpClient, Uri, Secret)> InsertNew(
        this HttpClient client, Secret original)
    {
        var response = await client.PostAsJsonAsync("secret", original);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var secret = await response.Content.ReadFromJsonAsync<Secret>(options);
        secret.Should().BeEquivalentTo(original);
        if (response.Headers.Location is not null)
        {
            return (client, response.Headers.Location, original);
        }

        // weird inverted name of an exception, I know
        throw new Xunit.Sdk.NotNullException();
    }

    public static async Task<(HttpClient, Uri, Secret)> VerifyItExists(this Task<(HttpClient, Uri, Secret)> chain)
        => await chain.Continue(async (client, location, _) =>
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, location));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        });

    public static async Task<(HttpClient, Uri, Secret)> VerifyItCanBeRead(this Task<(HttpClient, Uri, Secret)> chain)
        => await chain.Continue(async (client, location, original) =>
        {
            var response = await client.GetAsync(location);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var secret = await response.Content.ReadFromJsonAsync<Secret>(options);
            secret.Should().BeEquivalentTo(original);
        });

    public static async Task<(HttpClient, Uri, Secret)> DeleteIt(this Task<(HttpClient, Uri, Secret)> chain)
        => await chain.Continue(async (client, location, _) =>
        {
            var response = await client.DeleteAsync(location);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        });

    public static async Task VerifyItIsGone(this Task<(HttpClient, Uri, Secret)> chain)
        => await chain.Continue(async (client, location, _) =>
        {
            var response = await client.GetAsync(location);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        });

    private static async Task<(HttpClient, Uri, Secret)> Continue(
        this Task<(HttpClient, Uri, Secret)> previous, Func<HttpClient, Uri, Secret, Task> next)
    {
        var chain = await previous;
        var (client, location, secret) = chain;
        await next(client, location, secret);
        return chain;
    }
}