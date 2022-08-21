// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Validation;

namespace Api.Swashbuckle;

public class UntrustedIdentifierSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(UntrustedValue<Identifier>))
        {
            return;
        }

        schema.Type = "string";
        schema.Example = new OpenApiString("1337");
    }
}
