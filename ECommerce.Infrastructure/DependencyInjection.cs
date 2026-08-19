using ECommerce.Application.Brands;
using ECommerce.Application.Products;
using ECommerce.Application.Types;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Data.DbContexts;
using ECommerce.Infrastructure.Data.Interceptors;
using ECommerce.Infrastructure.persistence.Queries;
using ECommerce.Infrastructure.persistence.Seeding;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.StackExchangeRedis;
namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging();
        });



        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(
            typeof(IReadRepository<>),
            typeof(Repository<>));

        services.AddScoped(
            typeof(IRepository<>),
            typeof(Repository<>));

        services.AddScoped<IDataSeeder, ProductBrandSeeder>();
        services.AddScoped<IDataSeeder, ProductTypeSeeder>();

        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<IProductQueryService, ProductQueryService>();

        // Basket caching
        AddBasketCaching(services, config);

        return services;
    }


    // =========================
    // Basket Caching
    // =========================

    private static void AddBasketCaching(
        IServiceCollection services,
        IConfiguration config)
    {
        services
            .AddOptions<CacheEntryPolicy>("Basket")
            .Bind(config.GetSection("CachedAggregates:Basket"))
            .ValidateOnStart();

        services.AddSingleton<
            IValidateOptions<CacheEntryPolicy>,
            CacheEntryPolicyValidator>();

        var redisConnection =
            config.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddHybridCache();

        services.AddScoped(
            typeof(ICachedAggregateStore<>),
            typeof(HybridCacheAggregateStore<>));

        services.AddScoped<
            IBasketStore,
            HybridBasketStore>();
    }
}