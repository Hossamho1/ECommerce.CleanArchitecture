using ECommerce.Application.Types;
using ECommerce.Application.Types.Dtos;
using ECommerce.Infrastructure.Data.DbContexts;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ECommerce.Infrastructure.persistence.Queries;

public class ProductTypeQueryService(StoreDbContext dbContext) : ITypeQueryService
{
    public async Task<IReadOnlyList<GetAllTypeResponse>> GetAllTypesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Types
            .AsNoTracking()
            .ProjectToType<GetAllTypeResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<GetByIdTypeResponse?> GetByIdTypeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Types
            .AsNoTracking()
            .Where(t => t.Id == id)
            .ProjectToType<GetByIdTypeResponse>()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
