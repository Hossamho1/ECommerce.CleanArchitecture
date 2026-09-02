using ECommerce.API.Extensions;
using ECommerce.API.Models;
using ECommerce.API.Filters;
using ECommerce.UseCases.Identity.Commands.ConfirmEmail;
using ECommerce.UseCases.Identity.Commands.Login;
using ECommerce.UseCases.Identity.Commands.Logout;
using ECommerce.UseCases.Identity.Commands.RefreshToken;
using ECommerce.UseCases.Identity.Commands.Register;
using ECommerce.UseCases.Identity.Dtos;
// using ECommerce.UseCases.Messaging; // not required
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<EmailSentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<EmailSentResponse>>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        if (result.IsFailure)
            return Problem(result);

        return Success(result.Value, result.Value.Message);
    }

    [HttpPost("confirm-email")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmEmail(
        [FromBody] ConfirmEmailCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return FromResult(result, ApiMessages.EmailConfirmed);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
        [FromBody] LoginCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return FromResult(result, ApiMessages.LoggedIn);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(
        [FromBody] RefreshTokenCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return FromResult(result, ApiMessages.TokenRefreshed);
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Logout(
        [FromBody] LogoutCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return FromResult(result, ApiMessages.LoggedOut);
    }
}
