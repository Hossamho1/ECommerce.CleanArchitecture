using ECommerce.API.Extensions;
using ECommerce.API.Models;
using ECommerce.UseCases.Identity.Commands.AddUserAddress;
using ECommerce.UseCases.Identity.Commands.UpdateUserProfile;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Identity.Queries.GetCurrentUser;
using ECommerce.UseCases.Identity.Queries.GetUserAddresses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ApiControllerBase
{
    private readonly ISender _sender;

    public UserController(ISender sender)
    {
        _sender = sender;
    }

    // GET: api/users/me
    [HttpGet("me")]
    [ProducesResponseType(
        typeof(ApiResponse<UserProfileResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> GetCurrentUser(
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetCurrentUserQuery(), ct);

        return FromResult(result, ApiMessages.CurrentUserRetrieved);
    }


    // PUT: api/users/me
    [HttpPut("me")]
    [ProducesResponseType(
        typeof(ApiResponse<UserProfileResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> UpdateCurrentUser(
        [FromBody] UpdateUserProfileCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return FromResult(result, ApiMessages.UserProfileUpdated);
    }


    // GET: api/users/me/addresses
    [HttpGet("me/addresses")]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyList<UserAddressResponse>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserAddressResponse>>>> GetUserAddresses(
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetUserAddressesQuery(), ct);

        return FromResult(result, ApiMessages.UserAddressesRetrieved);
    }


    // POST: api/users/me/addresses
    [HttpPost("me/addresses")]
    [ProducesResponseType(
        typeof(ApiResponse<UserAddressResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserAddressResponse>>> AddUserAddress(
        [FromBody] AddUserAddressCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return FromResult(result, ApiMessages.UserAddressAdded);
    }
}
