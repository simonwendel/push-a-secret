using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Validation;

namespace Api.Swashbuckle;

public static class SwaggerGenOptionsExtensions
{
    public static void ConfigureSwaggerGen(this SwaggerGenOptions options, string apiVersion)
    {
        options.SwaggerDoc(apiVersion, new OpenApiInfo
        {
            Version = apiVersion,
            Title = "Push-a-Secret API",
            Description = "Secrets storage API serving Push-a-Secret clients."
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);

        options.OperationFilter<RemoveDefaultResponse>();
        options.SchemaFilter<SecretSchemaFilter>();

        options.MapType<UntrustedValue<string>>(() => new OpenApiSchema {Type = "string"});
    }
}
