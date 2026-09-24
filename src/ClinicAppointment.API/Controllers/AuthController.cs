using ClinicAppointment.API.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Features.Auth.Commands.Login;
using ClinicAppointment.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.API.Controllers;

/// <summary>
/// Creating an account and signing in to it. These are the only endpoints that may be
/// called without a token - everything else needs the JWT returned here.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Registers a Patient or Doctor account and returns it without the password.</summary>
    /// <remarks>
    /// A Patient account is also given the Patient record its appointments will belong to, so the
    /// same call can be used to start booking straight away. Passwords are stored hashed and are
    /// never read back. The email must not be registered yet.
    /// </remarks>
    /// <param name="request">Name, email, password (at least 8 characters) and role.</param>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="201">Account created. The response contains the account, not the password.</response>
    /// <response code="400">Name, email, password or role is missing or invalid.</response>
    /// <response code="409">An account with this email already exists.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Name,
            request.Email,
            request.Password,
            request.Role!.Value);

        var result = await _mediator.Send(command, cancellationToken);

        return this.ToCreatedActionResult(result);
    }

    /// <summary>Checks the credentials and returns a JWT to send as "Authorization: Bearer {token}".</summary>
    /// <remarks>
    /// The same answer is given for an unknown email and for a wrong password, so the endpoint
    /// cannot be used to find out which emails are registered.
    /// </remarks>
    /// <param name="request">Email and password.</param>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="200">Credentials are correct; the response carries the token, its expiry and the account.</response>
    /// <response code="400">Email or password is missing.</response>
    /// <response code="401">Email or password is incorrect.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password), cancellationToken);

        return this.ToActionResult(result);
    }
}
