using ECommerce.Domain;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public sealed class Repository<T>(StoreDbContext dbContext) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {

        return await _dbSet.FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        entity.MarkAsDeleted();

        _dbSet.Update(entity);
    }
}