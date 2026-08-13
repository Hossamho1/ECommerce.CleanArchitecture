using ECommerce.API.Models;
using ECommerce.Application.Products.Dtos;
using ECommerce.Application.Products.Queries;
using ECommerce.Application.Products.Enums;
using ECommerce.Domain.Common;
using MediatR;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ECommerce.API.Controllers;

public class ProductsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllProductResponse>>>> GetAll(CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllProductQuery(), ct);

        return FromResult(result);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllProductResponse>>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? brandId = null,
        [FromQuery] Guid? typeId = null,
        [FromQuery] ProductSortField sortBy = ProductSortField.Name,
        [FromQuery] bool sortDescending = false,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetPagedProductsQuery(pageNumber, pageSize, search, brandId, typeId, sortBy, sortDescending), ct);

        if (result.IsFailure)
            return Problem(result);

        var paged = result.Value;

        var pagination = new PaginationMeta(pageNumber, pageSize, paged.TotalCount);

        return Success(paged.Items, null, pagination);
    }

   [HttpGet("{id:guid}")] // Get API/Products/{id}
[ProducesResponseType(typeof(ApiResponse<GetByIdProductResponse>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetByIdProductResponse>>> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), ct);

        return FromResult(result);
    }
}
