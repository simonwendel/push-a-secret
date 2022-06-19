using Domain;
using Microsoft.AspNetCore.Mvc;
using Validation;

namespace Api;

[ApiController, Route("[controller]")]
public class SecretController : ControllerBase
{
    private readonly IIdentifierValidator idValidator;
    private readonly ISecretValidator secretValidator;
    private readonly IStore store;

    public SecretController(IStore store, IIdentifierValidator idValidator, ISecretValidator secretValidator)
    {
        this.store = store;
        this.idValidator = idValidator;
        this.secretValidator = secretValidator;
    }

    [HttpHead("{identifier}")]
    public IActionResult Head([FromRoute] UntrustedValue<string> identifier)
        => HandleRequestWithIdentifier<PeekRequest>(
            identifier,
            request => store.Peek(request).Result switch
            {
                Result.OK => Ok(),
                _ => NotFound()
            });

    [HttpGet("{identifier}")]
    public IActionResult Get([FromRoute] UntrustedValue<string> identifier)
        => HandleRequestWithIdentifier<ReadRequest>(
            identifier,
            request => store.Read(request) switch
            {
                (Result.OK, not null) response
                    => IsValidSecret(response.Secret!)
                        ? Ok(response.Secret)
                        : Conflict(),

                _ => NotFound()
            });

    [HttpDelete("{identifier}")]
    public IActionResult Delete([FromRoute] UntrustedValue<string> identifier)
        => HandleRequestWithIdentifier<DeleteRequest>(
            identifier,
            request => store.Delete(request).Result switch
            {
                Result.OK => NoContent(),
                _ => NotFound()
            });

    private IActionResult HandleRequestWithIdentifier<T>(
        UntrustedValue<string> identifier,
        Func<T, IActionResult> handle) where T : Identifier
    {
        try
        {
            var validatedId = idValidator.Validate(identifier);
            var request = Activator.CreateInstance(typeof(T), validatedId) as T;
            return handle(request!);
        }
        catch (ValidationException)
        {
            return BadRequest();
        }
    }

    private bool IsValidSecret(Secret secret)
    {
        try
        {
            secretValidator.Validate(new UntrustedValue<Secret>(secret));
            return true;
        }
        catch (ValidationException)
        {
            return false;
        }
    }
}
