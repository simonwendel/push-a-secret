// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swashbuckle;

public class RemoveDefaultResponse : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses.TryGetValue("200", out var response)
            && response.Description == "Success")
        {
            operation.Responses.Remove("200");
        }
    }
}
