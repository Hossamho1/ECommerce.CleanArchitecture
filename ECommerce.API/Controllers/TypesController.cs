using ECommerce.Application.Types.Dtos;
using ECommerce.Application.Types.Queries;
using Microsoft.AspNetCore.Mvc;
using ECommerce.API.Models;
using System.Threading;

namespace ECommerce.API.Controllers;

public class TypesController(GetAllTypesQuery getAllTypesQuery, GetByIdTypeQuery getByIdTypeQuery) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllTypeResponse>>>> GetAll(CancellationToken ct = default)
    {
        var result = await getAllTypesQuery.ExecuteAsync();

        return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GetByIdTypeResponse>>> Get(Guid id, CancellationToken ct = default)
    {
        var result = await getByIdTypeQuery.ExecuteAsync(id);

        return FromResult(result);
    }
}
