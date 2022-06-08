using Microsoft.AspNetCore.Mvc;
using Storage;
using Validation;

namespace Api;

[ApiController, Route("[controller]")]
public class SecretController : ControllerBase
{
    private readonly IIdentifierValidator validator;
    private readonly IStore store;

    public SecretController(IIdentifierValidator validator, IStore store)
    {
        this.validator = validator;
        this.store = store;
    }

    [HttpGet("{identifier}")]
    public IActionResult Get(UntrustedValue<string> identifier)
        => HandleValidatedRequest(identifier, validatedId =>
        {
            var request = new ReadRequest(validatedId);
            var response = store.Read(request);
            return response switch
            {
                (Result.OK, not null) => Ok(response.Secret),
                _ => NotFound(request)
            };
        });

    [HttpDelete("{identifier}")]
    public IActionResult Delete(UntrustedValue<string> identifier)
        => HandleValidatedRequest(identifier, validatedId =>
        {
            var request = new DeleteRequest(validatedId);
            var response = store.Delete(request);
            return response.Result switch
            {
                Result.OK => NoContent(),
                _ => NotFound(request)
            };
        });

    private IActionResult HandleValidatedRequest(UntrustedValue<string> identifier, Func<string, IActionResult> handle)
    {
        try
        {
            var validatedId = validator.Validate(identifier);
            return handle(validatedId);
        }
        catch (ValidationException)
        {
            return BadRequest("Malformed identifier.");
        }
    }
}
