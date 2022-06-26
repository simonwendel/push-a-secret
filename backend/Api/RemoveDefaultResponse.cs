using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api;

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
