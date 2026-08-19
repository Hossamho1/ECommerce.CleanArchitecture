using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Repositories;

public interface IBasketStore
{
    Task<Basket> GetOrCreateAsync(Guid buyerId,CancellationToken cancellationToken = default);

    Task SaveAsync(Basket basket, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid buyerId,CancellationToken cancellationToken=default);
}
