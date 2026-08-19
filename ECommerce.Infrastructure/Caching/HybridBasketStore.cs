using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Caching;

internal class HybridBasketStore(ICachedAggregateStore<Basket> store) : IBasketStore
{


    public Task<Basket> GetOrCreateAsync(Guid buyerId, CancellationToken ct = default)
     => store.GetOrCreateAsync(
         BuildCacheKey(buyerId),
         async cancel =>
         {
             var createResult = Basket.CreateEmpty(buyerId);

             if (createResult.IsFailure)
                 throw new Exception();

             return createResult.Value;
         },
         ct
     );
    public Task SaveAsync(Basket basket, CancellationToken ct = default)
        => store.SetAsync(BuildCacheKey(basket.BuyerId), basket, ct);

    public Task DeleteAsync(Guid buyerId, CancellationToken ct = default)
        => store.RemoveAsync(BuildCacheKey(buyerId), ct);

    private static string BuildCacheKey(Guid buyerId) => $"basket: {buyerId}";
}
