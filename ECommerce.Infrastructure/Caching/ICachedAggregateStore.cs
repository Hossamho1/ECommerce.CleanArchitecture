using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Caching;

public interface ICachedAggregateStore<T>
{
    Task<T?> GetAsync(string key, CancellationToken cancellationToken = default);
    Task<T?> GetOrCreateAsync(string key, Func<CancellationToken, Task<T?>> factory, CancellationToken cancellationToken = default);
    Task SetAsync(string key, T value, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

}
