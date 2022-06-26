using System.Reflection;
using Api;
using Domain;
using Microsoft.OpenApi.Models;
using Storage;
using Validation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(
    options => options.ModelBinderProviders.Insert(0, new UntrustedValueBinderProvider()));

builder.Services.AddEndpointsApiExplorer();

const string apiVersion = "v1";
builder.Services.AddSwaggerGen(options =>
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
    
    options.MapType<UntrustedValue<string>>(() => new OpenApiSchema {});
});

builder.Services
    .AddValidationModule()
    .AddStorageModule()
    .AddDomainModule();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
