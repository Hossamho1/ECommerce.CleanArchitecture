using ECommerce.Application.Products.Queries;
using ECommerce.Application.Types.Queries;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using ECommerce.Application.Brands.Queries;

namespace ECommerce.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Register MediatR handlers from this assembly (use Brand query as anchor)
        services.AddMediatR(typeof(GetAllBrandQuery).Assembly);

        // Keep existing non-MediatR use-case registrations if needed
        services.AddScoped<GetAllProductsQuery>();
        services.AddScoped<GetByIdProductsQuery>();

        services.AddScoped<GetAllTypesQuery>();
        services.AddScoped<GetByIdTypeQuery>();

        return services;
    }
}
