using ECommerce.Application.Brands;
using ECommerce.Application.Products;
using ECommerce.Application.Types;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Data.DbContexts;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.persistence.Queries;
using ECommerce.Infrastructure.persistence.Seeding;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.Infrastructure.Repositories;
using ECommerce.UseCases.Common.Settings;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.Infrastructure.Services;
using ECommerce.Application.Commons.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace ECommerce.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
                    sql.MigrationsHistoryTable("__ApplicationMigrationsHistory", "app"))
                .EnableSensitiveDataLogging();
        });

        services.AddDbContext<IdentityStoreDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
                    sql.MigrationsHistoryTable("__IdentityMigrationsHistory", "identity"))
                .EnableSensitiveDataLogging();
        });

        // Register identity application service
        services.AddScoped<IIdentityService, IdentityService>();




        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(
            typeof(IReadRepository<>),
            typeof(Repository<>));

        services.AddScoped(
            typeof(IRepository<>),
            typeof(Repository<>));

        services.AddScoped<IDataSeeder, ProductBrandSeeder>();
        services.AddScoped<IDataSeeder, ProductTypeSeeder>();
        services.AddScoped<IDataSeeder, IdentitySeeder>();
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<IProductQueryService, ProductQueryService>();
        services.AddScoped<ITypeQueryService, ProductTypeQueryService>();        
        services.AddScoped<IUserAddressService, UserAddressService>();
        // JWT generator
        services.AddScoped<IJwtTokenGenerator, Identity.JwtTokenGenerator>();
        AddBasketCaching(services, config);

        return services;
    }


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
            typeof(CachedAggregateStore<>));

        services.AddScoped<
            IBasketStore,
            HybridBasketStore>();
    }
}