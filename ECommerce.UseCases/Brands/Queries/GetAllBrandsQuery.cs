using ECommerce.Application.Brands.Dtos;
using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Brands.Queries;
public sealed class GetAllBrandsQuery(IBrandQueryService _queryService)
{
    public async Task<Result<IReadOnlyList<GetAllBrandResponse>>> ExecuteAsync()
    {
        var brands = await _queryService.GetAllBrandsAsync();

        return Result<IReadOnlyList<GetAllBrandResponse>>.Success(brands);
    }
}
