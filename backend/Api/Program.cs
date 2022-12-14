// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Net.Mime;
using Api;
using Api.Swashbuckle;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Storage;
using Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(prefix: "API_");
builder.Services.AddSingleton(
    builder.Configuration.GetSection("Storage").Get<StorageConfiguration>());

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(
    options =>
    {
        options.ModelBinderProviders.Insert(0, new UntrustedValueBinderProvider());
        options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.ConfigureSwaggerGen(apiVersion: "v1"));

var corsPolicy = builder.AddCustomCorsPolicy();

builder.Services
    .AddValidationModule()
    .AddStorageModule()
    .AddDomainModule();

var app = builder.Build();
app.UseMiddleware<ServerErrorMiddleware>();
app.UseSwagger();
if(builder.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseCors(corsPolicy);
app.MapControllers();

app.Run();
