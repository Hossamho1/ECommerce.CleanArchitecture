using ECommerce.Application.Brands.Dtos;
using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Brands.Queries;

public sealed class GetByIdBrandQuery(IBrandQueryService _queryService)
{
    public async Task<Result<GetByIdBrandResponse>> ExecuteAsync(Guid id)
    {
        var brand = await _queryService.GetByIdBrandAsync(id);

        if (brand == null)
        {
            return Result<GetByIdBrandResponse>.Failure(BrandErrors.NotFound);
        }

        return Result<GetByIdBrandResponse>.Success(brand);
    }
}
