using ECommerce.API.Models;
using ECommerce.Application.Products.Dtos;
using ECommerce.Application.Products.Queries;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ECommerce.API.Controllers;

public class ProductsController(GetAllProductsQuery getAllProductsQuery, GetByIdProductsQuery getByIdProductsQuery) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllProductResponse>>>> GetAll(CancellationToken ct = default)
    {
        var result = await getAllProductsQuery.ExecuteAsync(ct);

        return FromResult(result);
    }

   [HttpGet("{id:guid}")] // Get API/Products/{id}
[ProducesResponseType(typeof(ApiResponse<GetByIdProductResponse>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
public async Task<ActionResult<ApiResponse<GetByIdProductResponse>>> GetById(Guid id, CancellationToken ct = default)
{
    var result = await getByIdProductsQuery.ExecuteAsync(id, ct);

    return FromResult(result);
}
}
