using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Api;

/// <summary>
/// Middleware that intercepts any exceptions in the pipeline.
/// </summary>
/// <remarks>
/// We take care to generate <see cref="ProblemDetails"/> as per standard and return along with headers matching
/// the responses from the built-in <see cref="ControllerBase"/> methods returning <see cref="IActionResult"/>.
/// This swallows any exception output and ensures all errors look the same.
/// </remarks>
public class ServerErrorMiddleware
{
    private readonly RequestDelegate _next;

    public ServerErrorMiddleware(RequestDelegate next) 
        => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            var response = context.Response;
            response.Clear();
            response.StatusCode = 500;
            var problemDetails = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An error occurred while processing your request.",
                Status = (int) HttpStatusCode.InternalServerError
            };

            var traceId = Activity.Current?.Id;
            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }

            await response.WriteAsJsonAsync(problemDetails);
        }
    }
}
