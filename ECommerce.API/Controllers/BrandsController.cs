using ECommerce.Application.Brands.Dtos;
using ECommerce.Application.Brands.Enums;
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

    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllBrandResponse>>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] BrandSortField sortBy = BrandSortField.Name,
        [FromQuery] bool sortDescending = false,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetPagedBrandsQuery(pageNumber, pageSize, search, sortBy, sortDescending), ct);

        if (result.IsFailure)
            return Problem(result);

        var paged = result.Value;

        var pagination = new PaginationMeta(pageNumber, pageSize, paged.TotalCount);

        return Success(paged.Items, null, pagination);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GetByIdBrandResponse>>> Get(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new ECommerce.Application.Brands.Queries.GetByIdBrandQuery(id), ct);

        return FromResult(result);
    }
}
