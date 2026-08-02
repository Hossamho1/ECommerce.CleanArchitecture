using ECommerce.Application.Brands.Dtos;
using ECommerce.Application.Brands.Queries;
using Microsoft.AspNetCore.Mvc;
using ECommerce.API.Models;
using System.Threading;

namespace ECommerce.API.Controllers;

public class BrandsController(GetAllBrandsQuery getAllBrands, GetByIdBrandQuery getBrandsById) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllBrandResponse>>>> GetAllBrands(CancellationToken cancellationToken = default)
    {
        var result = await getAllBrands.ExecuteAsync(cancellationToken);

        return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GetByIdBrandResponse>>> Get(Guid id, CancellationToken ct = default)
    {
        var result = await getBrandsById.ExecuteAsync(id);

        return FromResult(result);
    }
}
