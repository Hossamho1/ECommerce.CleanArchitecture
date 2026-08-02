using ECommerce.Application.Products.Dtos;
using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ECommerce.Application.Products.Queries;
public sealed class GetAllProductsQuery(IProductQueryService _queryService)
{

    public async Task<Result<IReadOnlyList<GetAllProductResponse>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var products = await _queryService.GetAllProductsAsync();

        return Result<IReadOnlyList<GetAllProductResponse>>
            .Success(products);
    }
}