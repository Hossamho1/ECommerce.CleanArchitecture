using ECommerce.Application.Types.Dtos;
using ECommerce.Application.Types.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ECommerce.API.Models;
using System.Threading;

namespace ECommerce.API.Controllers;

public class TypesController(IMediator mediator) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllTypeResponse>>>> GetAll(CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllTypeQuery(), ct);

        return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GetByIdTypeResponse>>> Get(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetTypeByIdQuery(id), ct);

        return FromResult(result);
    }
}
