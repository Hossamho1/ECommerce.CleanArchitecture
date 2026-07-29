namespace ECommerce.Domain.Repositories;

/// <summary>
/// Coordinates one or more repositories sharing a single DbContext / transaction.
/// SaveChangesAsync is the only place a transaction is committed - repositories
/// never call SaveChanges themselves.
/// </summary>
public interface IUnitOfWork
{
    IRepository<T, TKey> Repository<T, TKey>() where T : class, IEntity<TKey> where TKey : IEquatable<TKey>;

    /// <summary>Persists all tracked changes across every repository obtained from
    /// this Unit of Work, in a single round trip / transaction.</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>Explicit transaction for cases needing more than SaveChangesAsync's
    /// implicit transaction - e.g. mixing ExecuteUpdateAsync bulk ops with tracked
    /// entity changes that must commit or roll back together.</summary>
    Task<IDisposable> BeginTransactionAsync(CancellationToken ct = default);

    Task CommitTransactionAsync(CancellationToken ct = default);

    Task RollbackTransactionAsync(CancellationToken ct = default);
}