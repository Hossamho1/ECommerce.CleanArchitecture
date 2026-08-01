using ECommerce.Application.Brands.Dtos;

namespace ECommerce.Application.Brands;

public interface IBrandQueryService
{
    Task<IReadOnlyList<GetAllBrandResponse>> GetAllBrandsAsync(CancellationToken cancellationToken = default);
    Task<GetByIdBrandResponse?> GetByIdBrandAsync(Guid id, CancellationToken cancellationToken = default);
}
