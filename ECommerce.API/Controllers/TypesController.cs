using ECommerce.Application.Types.Dtos;
using ECommerce.Application.Types.Enums;
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

    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllTypeResponse>>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] TypeSortField sortBy = TypeSortField.Name,
        [FromQuery] bool sortDescending = false,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetPagedTypesQuery(pageNumber, pageSize, search, sortBy, sortDescending), ct);

        if (result.IsFailure)
            return Problem(result);

        var paged = result.Value;

        var pagination = new PaginationMeta(pageNumber, pageSize, paged.TotalCount);

        return Success(paged.Items, null, pagination);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GetByIdTypeResponse>>> Get(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetTypeByIdQuery(id), ct);

        return FromResult(result);
    }
}
