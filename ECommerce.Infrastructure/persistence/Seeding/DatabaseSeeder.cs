using ECommerce.Infrastructure.Data.DbContexts;
using ECommerce.Infrastructure.persistence.Seeding;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public sealed class DatabaseSeeder(StoreDbContext dbContext,
    IEnumerable<IDataSeeder> seeders)
{
    public async Task SeedAll(CancellationToken ct = default)
    {
        foreach (var seeder in seeders.OrderBy(s => s.Order))
        {
            await seeder.SeedAsync(ct);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}