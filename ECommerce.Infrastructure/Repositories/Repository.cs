using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Specifications;
using ECommerce.Infrastructure.Data.DbContexts;
using ECommerce.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public sealed class Repository<T>(StoreDbContext dbContext)
    : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet, specification).FirstOrDefaultAsync(ct);
    }

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(
        ISpecification<T, TResult> specification,
        CancellationToken ct = default)
        where TResult : class
    {
        return await SpecificationEvaluator.GetQuery<T, TResult>(_dbSet, specification).FirstOrDefaultAsync(ct);
    }

    public async Task<T> SingleAsync(ISpecification<T> specification, CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet, specification).SingleAsync(ct);
    }

    public async Task<TResult> SingleAsync<TResult>(
        ISpecification<T, TResult> specification,
        CancellationToken ct = default)
        where TResult : class
    {
        return await SpecificationEvaluator.GetQuery<T, TResult>(_dbSet, specification).SingleAsync(ct);
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet, specification).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        ISpecification<T, TResult> specification,
        CancellationToken ct = default)
        where TResult : class
    {
        return await SpecificationEvaluator.GetQuery<T, TResult>(_dbSet, specification).ToListAsync(ct);
    }

    public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetCountQuery(_dbSet, specification).CountAsync(ct);
    }

    public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetCountQuery(_dbSet, specification).AnyAsync(ct);
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
        => entity.MarkAsDeleted();
}