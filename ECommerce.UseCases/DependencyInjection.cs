using ECommerce.Application.Products.Queries;
using ECommerce.Application.Brands.Queries;
using ECommerce.Application.Types.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Products
        services.AddScoped<GetAllProductsQuery>();
        services.AddScoped<GetByIdProductsQuery>();

        // Brands
        services.AddScoped<GetAllBrandsQuery>();
        services.AddScoped<GetByIdBrandQuery>();

        // Types
        services.AddScoped<GetAllTypesQuery>();
        services.AddScoped<GetByIdTypeQuery>();

        return services;
    }
}
