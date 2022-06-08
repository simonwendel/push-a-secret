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

    [HttpHead("{identifier}")]
    public IActionResult Head(UntrustedValue<string> identifier)
        => ValidateRequestWithIdentifier<PeekRequest>(
            identifier,
            request => store.Peek(request).Result switch
            {
                Result.OK => Ok(),
                _ => NotFound()
            });

    [HttpGet("{identifier}")]
    public IActionResult Get(UntrustedValue<string> identifier)
        => ValidateRequestWithIdentifier<ReadRequest>(
            identifier,
            request => store.Read(request) switch
            {
                (Result.OK, not null) response => Ok(response.Secret),
                _ => NotFound()
            });

    [HttpDelete("{identifier}")]
    public IActionResult Delete(UntrustedValue<string> identifier)
        => ValidateRequestWithIdentifier<DeleteRequest>(
            identifier,
            request => store.Delete(request).Result switch
            {
                Result.OK => NoContent(),
                _ => NotFound()
            });

    private IActionResult ValidateRequestWithIdentifier<T>(
        UntrustedValue<string> identifier,
        Func<T, IActionResult> handle) where T : Identifier
    {
        try
        {
            var validatedId = validator.Validate(identifier);
            var request = Activator.CreateInstance(typeof(T), validatedId) as T;
            return handle(request!);
        }
        catch (ValidationException)
        {
            return BadRequest();
        }
    }
}
