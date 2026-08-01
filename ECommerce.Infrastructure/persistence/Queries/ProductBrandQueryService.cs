using ECommerce.Application.Brands;
using ECommerce.Application.Brands.Dtos;
using ECommerce.Infrastructure.Data.DbContexts;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ECommerce.Infrastructure.persistence.Queries;

public class ProductBrandQueryService(StoreDbContext dbContext) : IBrandQueryService
{
    public async Task<IReadOnlyList<GetAllBrandResponse>> GetAllBrandsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Brands
            .AsNoTracking()
            .ProjectToType<GetAllBrandResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<GetByIdBrandResponse?> GetByIdBrandAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Brands
            .AsNoTracking()
            .Where(b => b.Id == id)
            .ProjectToType<GetByIdBrandResponse>()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
