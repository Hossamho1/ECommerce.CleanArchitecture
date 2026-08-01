using ECommerce.Application.Products.Dtos;
using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Products.Queries
{
    public sealed class GetByIdProductsQuery(IProductQueryService _queryService)
    {
        public async Task<Result<IReadOnlyList<GetByIdProductResponse>>> ExecuteAsync(Guid id)
        {
            var product = await _queryService.GetByIdProductResponseAsync(id);

            if(product == null)
            {
                return Result<IReadOnlyList<GetByIdProductResponse>>
                    .Failure(ProductErrors.NotFound);
            }   

            return Result<IReadOnlyList<GetByIdProductResponse>>
                .Success(new List<GetByIdProductResponse> { product });
        }
    }
}
