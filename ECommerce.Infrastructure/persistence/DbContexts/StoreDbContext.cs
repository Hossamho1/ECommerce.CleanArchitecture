using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.DbContexts;

public class StoreDbContext(DbContextOptions<StoreDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductBrand> Brands => Set<ProductBrand>();

    public DbSet<ProductType> Types => Set<ProductType>();

    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
                 typeof(StoreDbContext).Assembly,
                 type => type.Namespace == "ECommerce.Infrastructure.Persistence.Configurations");

        base.OnModelCreating(modelBuilder);
    }
}