using ECommerce.Application.Products.Dtos;
using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ECommerce.Application.Products.Queries
{
    public sealed class GetByIdProductsQuery(IProductQueryService _queryService)
    {
        public async Task<Result<GetByIdProductResponse>> ExecuteAsync(Guid id, CancellationToken ct)
        {
            var product = await _queryService.GetByIdProductResponseAsync(id, ct);

            if (product == null)
            {
                return Result<GetByIdProductResponse>
                    .Failure(ProductErrors.NotFound);
            }

            return Result<GetByIdProductResponse>.Success(product);
        }
    }
}
