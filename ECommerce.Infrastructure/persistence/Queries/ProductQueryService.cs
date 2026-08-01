using ECommerce.Application.Products;
using ECommerce.Application.Products.Dtos;
using ECommerce.Infrastructure.Data.DbContexts;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.persistence.Queries;

public class ProductQueryService(StoreDbContext dbContext) : IProductQueryService
{
    public async Task<IReadOnlyList<GetAllProductResponse>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .ProjectToType<GetAllProductResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<GetByIdProductResponse?> GetByIdProductResponseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products.AsNoTracking()
            .Where(p => id == p.Id)
            .ProjectToType<GetByIdProductResponse>()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
