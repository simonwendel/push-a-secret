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
    {
        try
        {
            var id = validator.Validate(identifier);
            var request = new ReadRequest(id);
            var response = store.Read(request);
            return response switch
            {
                (Result.OK, not null) => Ok(response.Secret),
                _ => NotFound(request)
            };
        }
        catch (ValidationException)
        {
            return BadRequest("Malformed identifier.");
        }
    }

    [HttpDelete("{identifier}")]
    public IActionResult Delete(UntrustedValue<string> identifier)
    {
        try
        {
            var id = validator.Validate(identifier);
            var request = new DeleteRequest(id);
            var response = store.Delete(request);
            return response.Result switch
            {
                Result.OK => NoContent(),
                _ => NotFound(request)
            };
        }
        catch (ValidationException)
        {
            return BadRequest("Malformed identifier.");
        }
    }
}
