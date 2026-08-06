using ECommerce.Domain.Entities;
using ECommerce.Domain.Specifications;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace ECommerce.Domain.Repositories;

public interface IReadRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken ct = default);

    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default)
        where TResult : class;

    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken ct = default);

    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default)
        where TResult : class;

    Task<int> CountAsync(ISpecification<T> specification, CancellationToken ct = default);

    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken ct = default);

}