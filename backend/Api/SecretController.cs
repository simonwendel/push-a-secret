using System.Net.Mime;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Validation;

namespace Api;

[ApiController]
[Route("[controller]")]
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

    /// <summary>
    /// Check for secret in persistent storage.
    /// </summary>
    /// <remarks>
    /// The response will never have a body. Issuing a HTTP GET request instead using the same
    /// identifier will return the secret itself.
    /// </remarks>
    /// <param name="identifier">String identifier of the secret to look up.</param>
    /// <returns>Empty response.</returns>
    /// <response code="200">A secret matching supplied identifier can be retrieved from storage.</response>
    /// <response code="400">Supplied identifier has incorrect format, or request is otherwise invalid.</response>
    /// <response code="404">No secret in storage matches supplied identifier.</response>
    [HttpHead("{identifier}")]
    [ProducesResponseType(200, Type = default!)]
    [ProducesResponseType(400, Type = default!)]
    [ProducesResponseType(404, Type = default!)]
    public IActionResult Head([FromRoute] UntrustedValue<string> identifier)
        => HandleRequestWithIdentifier(
            identifier,
            validated => store.Peek(validated) switch
            {
                Result.OK => Ok(),
                _ => NotFound()
            });

    /// <summary>
    /// Retrieve a secret from persistent storage.
    /// </summary>
    /// <param name="identifier">String identifier of the secret to look up.</param>
    /// <returns>Secret object, if successful, otherwise empty response.</returns>
    /// <response code="200">A secret matching supplied identifier is retrieved from storage.</response>
    /// <response code="400">Supplied identifier has incorrect format, or request is otherwise invalid.</response>
    /// <response code="404">No secret in storage matches supplied identifier.</response>
    /// <response code="409">A secret in storage matching supplied identifier does not pass validation.</response>
    [HttpGet("{identifier}")]
    [ProducesResponseType(200, Type = typeof(Secret))]
    [ProducesResponseType(400, Type = default!)]
    [ProducesResponseType(404, Type = default!)]
    [ProducesResponseType(409, Type = default!)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult Get([FromRoute] UntrustedValue<string> identifier)
        => HandleRequestWithIdentifier(
            identifier,
            validated => store.Read(validated) switch
            {
                (Result.OK, not null) response
                    => IsValidSecret(response.Secret!)
                        ? Ok(response.Secret)
                        : Conflict(),

                _ => NotFound()
            });

    /// <summary>
    /// Save a secret to persistent storage.
    /// </summary>
    /// <param name="secret">A secret to persist to storage.</param>
    /// <returns>Secret object, if successful, otherwise empty response.</returns>
    /// <response code="201">A secret is persisted to storage. Location header contains resource URL.</response>
    /// <response code="400">Supplied secret has incorrect format, or request is otherwise invalid.</response>
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Secret))]
    [ProducesResponseType(400, Type = default!)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult Post([FromBody] UntrustedValue<Secret> secret)
    {
        try
        {
            var validated = secretValidator.Validate(secret);
            return store.Create(validated) switch
            {
                (Result.OK, not null) response => Created(ConstructResourceUrl(response.Identifier!), validated),
                _ => StatusCode(500) // probably only happens if the db is down, we needn't add it to docs
            };
        }
        catch (ValidationException)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Remove a secret from persistent storage.
    /// </summary>
    /// <param name="identifier">String identifier of the secret to delete.</param>
    /// <returns>Empty response.</returns>
    /// <response code="204">A secret matching supplied identifier is removed from storage.</response>
    /// <response code="400">Supplied identifier has incorrect format, or request is otherwise invalid.</response>
    /// <response code="404">No secret in storage matches supplied identifier.</response>
    [HttpDelete("{identifier}")]
    [ProducesResponseType(204, Type = default!)]
    [ProducesResponseType(400, Type = default!)]
    [ProducesResponseType(404, Type = default!)]
    public IActionResult Delete([FromRoute] UntrustedValue<string> identifier)
        => HandleRequestWithIdentifier(
            identifier,
            validated => store.Delete(validated) switch
            {
                Result.OK => NoContent(),
                _ => NotFound()
            });

    private IActionResult HandleRequestWithIdentifier(
        UntrustedValue<string> input,
        Func<Identifier, IActionResult> handle)
    {
        try
        {
            var validatedValue = idValidator.Validate(input);
            var identifier = new Identifier(validatedValue);
            return handle(identifier ?? throw new InvalidOperationException());
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

    private string ConstructResourceUrl(Identifier identifier)
        => Url?.Action(nameof(Get), new {identifier = identifier.Value}) ?? string.Empty;
}
