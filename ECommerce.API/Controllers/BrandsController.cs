using ECommerce.Application.Brands.Dtos;
using ECommerce.Application.Brands.Queries;
using Microsoft.AspNetCore.Mvc;
using ECommerce.API.Models;
using System.Threading;

using MediatR;

namespace ECommerce.API.Controllers;

public class BrandsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllBrandResponse>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ECommerce.Application.Brands.Queries.GetAllBrandQuery(), cancellationToken);

        return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GetByIdBrandResponse>>> Get(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new ECommerce.Application.Brands.Queries.GetBrandByIdQuery(id), ct);

        return FromResult(result);
    }
}
