using ECommerce.Application.Products;
using ECommerce.Application.Products.Dtos;
using ECommerce.Application.Products.Queries;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data.DbContexts;
using ECommerce.Infrastructure.Data.Interceptors;
using ECommerce.Infrastructure.persistence.Queries;
using ECommerce.Infrastructure.persistence.Seeding;
using ECommerce.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ECommerce.Infrastructure.Repositories;

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
        services.AddScoped<IProductQueryService, ProductQueryService>();

        // Unit of Work and generic repository
        services.AddScoped<ECommerce.Domain.Repositories.IUnitOfWork, ECommerce.Domain.Repositories.UnitOfWork>();

        // Brand and Type query service registrations
        services.AddScoped< ECommerce.Application.Brands.IBrandQueryService, ECommerce.Infrastructure.persistence.Queries.ProductBrandQueryService>();
        services.AddScoped< ECommerce.Application.Types.ITypeQueryService, ECommerce.Infrastructure.persistence.Queries.ProductTypeQueryService>();

        return services;
    }
}