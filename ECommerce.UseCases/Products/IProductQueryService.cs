

using ECommerce.Application.Products.Dtos;

namespace ECommerce.Application.Products;

public interface IProductQueryService
{
    Task<IReadOnlyList<GetAllProductResponse>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<GetByIdProductResponse?> GetByIdProductResponseAsync(Guid id, CancellationToken cancellationToken = default);
}
