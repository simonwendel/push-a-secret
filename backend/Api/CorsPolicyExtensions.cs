namespace Api;

public static class CorsPolicyExtensions
{
    public static string AddCustomCorsPolicy(this WebApplicationBuilder builder)
    {
        string[] GetSection(string key)
        {
            var config = builder!.Configuration.GetSection(key);
            return config.Get<string[]>() ?? Array.Empty<string>();
        }

        var allowOrigins = GetSection("Api:CORS:AllowOrigins");
        var allowMethods = GetSection("Api:CORS:AllowMethods");
        var exposeHeaders = GetSection("Api:CORS:ExposeHeaders");
        if (!allowOrigins.Any() || !allowMethods.Any() || !exposeHeaders.Any())
        {
            throw new InvalidOperationException("CORS not configured correctly.");
        }

        const string corsPolicyName = "CustomPolicy";
        builder.Services.AddCors(options => options.AddPolicy(
            corsPolicyName,
            policy => policy
                .WithOrigins(allowOrigins)
                .WithMethods(allowMethods)
                .WithExposedHeaders(exposeHeaders)
                .Build()));
        return corsPolicyName;
    }
}
