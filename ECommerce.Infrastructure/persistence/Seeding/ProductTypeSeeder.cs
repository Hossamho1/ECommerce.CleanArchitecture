using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data.DbContexts;
using ECommerce.Infrastructure.persistence.Seeding.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.persistence.Seeding;

public class ProductTypeSeeder(StoreDbContext dbContext ): IDataSeeder
{

    public int Order => 2;

    public async Task SeedAsync(CancellationToken ct = default)
        => await JsonSeeder.SeedIfEmpty<ProductType, ProductTypeSeedModel>(
            dbContext.Types, "brands.json", b => ProductType.Create(b.Id, b.Name), ct);
}
