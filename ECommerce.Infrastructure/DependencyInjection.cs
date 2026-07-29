using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data.DbContexts;
using ECommerce.Infrastructure.Data.Interceptors;
using ECommerce.Infrastructure.persistence.Seeding;
using ECommerce.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {

        services.AddSingleton<AuditColumnsInterceptor>();
        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"))
            .EnableSensitiveDataLogging();
        });

        services.AddScoped<IDataSeeder, ProductBrandSeeder>();
        services.AddScoped<IDataSeeder, ProductTypeSeeder>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}