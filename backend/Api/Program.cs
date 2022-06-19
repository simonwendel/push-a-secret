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
builder.Services.AddSwaggerGen(c =>
    c.MapType<UntrustedValue<string>>(() => new OpenApiSchema {Type = "string"}));

builder.Services
    .AddValidationModule()
    .AddStorageModule()
    .AddDomainModule();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
