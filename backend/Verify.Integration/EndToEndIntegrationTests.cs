using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Verify.Integration;

[Collection("Verify.Integration")]
public class EndToEndIntegrationTests
{
    private readonly Secret original;
    private readonly HttpClient client;

    public EndToEndIntegrationTests()
    {
        var app = new ApiApplication();
        client = app.CreateClient();
        original = new("A128GCM", "xMY6HokU51VT8g02", "zb8HP7LFqYr+1fWZA5ZFAfIHz3Y=");
    }

    [Fact]
    internal void RoundTrip_WhenRunAgainstApi_WorksAsIntended()
        => client
            .InsertNewSecret(original)
            .VerifyItExists()
            .VerifyItCanBeRead()
            .DeleteTheSecret()
            .VerifyItIsGone();
}

internal static class EndToEndIntegrationTestExtensions
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static (HttpClient, Uri, Secret) InsertNewSecret(
        this HttpClient client, Secret original)
    {
        var response = client.PostAsJsonAsync("secret", original).Result;
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var secret = response.Content.ReadFromJsonAsync<Secret>(options).Result;
        secret.Should().BeEquivalentTo(original);
        if (response.Headers.Location is not null)
        {
            return (client, response.Headers.Location, original);
        }

        // weird inverted name of an exception, I know
        throw new Xunit.Sdk.NotNullException();
    }

    public static (HttpClient, Uri, Secret) VerifyItExists(
        this (HttpClient, Uri, Secret) chain)
    {
        var (client, location, _) = chain;
        var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, location)).Result;
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return chain;
    }

    public static (HttpClient, Uri, Secret) VerifyItCanBeRead(
        this (HttpClient, Uri, Secret) chain)
    {
        var (client, location, original) = chain;
        var response = client.GetAsync(location).Result;
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var secret = response.Content.ReadFromJsonAsync<Secret>(options).Result;
        secret.Should().BeEquivalentTo(original);
        return chain;
    }

    public static (HttpClient, Uri, Secret) DeleteTheSecret(
        this (HttpClient, Uri, Secret) chain)
    {
        var (client, location, _) = chain;
        var response = client.DeleteAsync(location).Result;
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        return chain;
    }

    public static void VerifyItIsGone(this (HttpClient, Uri, Secret) chain)
    {
        var (client, location, _) = chain;
        var response = client.GetAsync(location).Result;
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

internal class ApiApplication : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IRepository));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddSingleton<IRepository>(_ => TestMongoDbRepositoryFactory.Build(cleanAll: true));
        });
    }
}
