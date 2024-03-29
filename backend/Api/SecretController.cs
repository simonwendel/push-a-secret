﻿// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

using Domain;
using Microsoft.AspNetCore.Mvc;
using Validation;

namespace Api;

[ApiController]
[Route("[controller]")]
[ProducesErrorResponseType(typeof(ProblemDetails))]
public class SecretController : ControllerBase
{
    private readonly IValidator validator;
    private readonly IStore store;

    public SecretController(IStore store, IValidator validator)
    {
        this.store = store;
        this.validator = validator;
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
    public IActionResult Head([FromRoute] UntrustedValue<Identifier> identifier)
        => HandleRequestWith(
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
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public IActionResult Get([FromRoute] UntrustedValue<Identifier> identifier)
        => HandleRequestWith(
            identifier,
            validated => store.Read(validated) switch
            {
                (Result.OK, not null) response
                    => IsValidSecret(response.Secret)
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
    [ProducesResponseType(400)]
    public IActionResult Post([FromBody] UntrustedValue<Secret> secret)
        => HandleRequestWith(
            secret,
            validated => store.Create(validated) switch
            {
                (Result.OK, not null) response
                    => Created(
                        ConstructResourceUrl(response.Identifier),
                        validated),

                _ => StatusCode(500) // probably only happens if the db is down, we needn't add it to docs
            });

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
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult Delete([FromRoute] UntrustedValue<Identifier> identifier)
        => HandleRequestWith(
            identifier,
            validated => store.Delete(validated) switch
            {
                Result.OK => NoContent(),
                _ => NotFound()
            });

    private IActionResult HandleRequestWith<T>(UntrustedValue<T> input, Func<T, IActionResult> handle) where T : notnull
        => TryValidate(input, out var validated) && validated is not null
            ? handle(validated)
            : BadRequest();

    private bool IsValidSecret(Secret? secret)
        => secret is not null && TryValidate(new UntrustedValue<Secret>(secret), out _);

    private bool TryValidate<T>(UntrustedValue<T> input, out T? validated) where T : notnull
    {
        try
        {
            validated = validator.Validate(input)
                        ?? throw new ValidationException();
            return true;
        }
        catch (ValidationException)
        {
            validated = default;
            return false;
        }
    }

    private string ConstructResourceUrl(Identifier? identifier)
        => identifier switch
        {
            (not null) => Url?.Action(nameof(Get), new {identifier = identifier.Value}),
            _ => null
        } ?? string.Empty;
}
