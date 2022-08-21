// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swashbuckle;

public class ProblemDetailsSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(ProblemDetails))
        {
            schema.Example = new OpenApiObject
            {
                ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc2324#section-2.3.2"),
                ["title"] = new OpenApiString("I'm a teapot"),
                ["status"] = new OpenApiInteger(418),
                ["traceId"] = new OpenApiString("00-40688664777a1c9d9ffdea61601410a2-a3b19f8cd1b1ce2c-00")
            };
        }
    }
}
