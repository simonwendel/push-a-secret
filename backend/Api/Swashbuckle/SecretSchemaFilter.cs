// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swashbuckle;

public class SecretSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Secret))
        {
            schema.Example = new OpenApiObject
            {
                ["algorithm"] = new OpenApiString("A128GCM"),
                ["ciphertext"] = new OpenApiString("BbrE2kT+87Zg+2JHSa5/4FdoEAl6X4JoaQPFdaRbcmRA5A=="),
                ["iv"] = new OpenApiString("nBtsLdhcT/0O+Pd/"),
                ["ttl"] = new OpenApiInteger(1)
            };
        }
    }
}
